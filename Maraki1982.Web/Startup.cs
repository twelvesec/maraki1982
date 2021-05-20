using Microsoft.AspNetCore.HttpOverrides;
using Maraki1982.Web.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Maraki1982.Core.DAL;
using Maraki1982.Core.Models.Enum;
using Maraki1982.Core.VendorApi;
using Maraki1982.Web.Utilities.Implementations;
using Maraki1982.Web.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Maraki1982.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserUtility, UserUtility>();
            services.AddScoped<IEmailUtility, EmailUtility>();
            services.AddScoped<IDriveUtility, DriveUtility>();

            #region Multiple Implementations with delegates

            services.AddTransient<MsGraphApi>();
            services.AddTransient<GoogleApi>();

            services.AddTransient<Func<VendorEnum, IExternalApi>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case VendorEnum.Microsoft:
                        return serviceProvider.GetService<MsGraphApi>();
                    case VendorEnum.Google:
                        return serviceProvider.GetService<GoogleApi>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

            #endregion

            services.AddControllersWithViews();

            switch (Configuration["General:DbType"])
            {
                case "SQLite":
                    services.AddDbContext<OAuthServerContext>(options =>
                            options.UseSqlite(
                                Configuration.GetConnectionString("OAuthServerContextConnection"), b => b.MigrationsAssembly("Maraki1982.Web")));
                    break;
                case "SQLServer":
                    services.AddDbContext<OAuthServerContext>(options =>
                            options.UseSqlServer(
                                Configuration.GetConnectionString("OAuthServerContextConnection"), b => b.MigrationsAssembly("Maraki1982.Web")));
                    break;
                case "PostgreSQL":
                    services.AddDbContext<OAuthServerContext>(options =>
                            options.UseNpgsql(
                                Configuration.GetConnectionString("OAuthServerContextConnection"), b => b.MigrationsAssembly("Maraki1982.Web")));
                    break;
                default:
                    break;
            }

            services.AddDefaultIdentity<UserModel>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.SignIn.RequireConfirmedAccount = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<OAuthServerContext>()
                .AddDefaultTokenProviders();

            services.Configure<PasswordHasherOptions>(options => options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3);

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("logs/maraki1982-{Date}.txt", isJson: true);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
            }
            else
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
                app.UseMiddleware<ExceptionMiddleware>();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
