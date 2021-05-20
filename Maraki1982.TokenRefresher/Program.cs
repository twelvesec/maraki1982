using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Maraki1982.Core.DAL;
using Maraki1982.Core.Models.Enum;
using Maraki1982.Core.VendorApi;
using System;
using System.Linq;
using System.Threading;

namespace Maraki1982.TokenRefresher
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false)
                    .Build();

                DbContextOptionsBuilder<OAuthServerContext> optionsBuilder = GetConnectionOptions(configuration);

                do
                {
                    Console.WriteLine("Updating user tokens...");

                    using (var dbContext = new OAuthServerContext(optionsBuilder.Options))
                    {
                        var users = dbContext.Users.OrderBy(x => x.Vendor).ToList();
                        IExternalApi msGraphApi = new MsGraphApi(dbContext, configuration);
                        IExternalApi googleApi = new GoogleApi(dbContext, configuration);
                        users.ForEach(x =>
                        {
                            switch (x.Vendor)
                            {
                                case VendorEnum.Microsoft:
                                    msGraphApi.RefreshToken(x);
                                    break;
                                case VendorEnum.Google:
                                    googleApi.RefreshToken(x);
                                    break;
                                default:
                                    break;
                            }
                        });
                    }

                    Console.WriteLine("Go to sleep for 55 minutes.");
                    Thread.Sleep(TimeSpan.FromMinutes(55));
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(string.Format("Message: {0}", ex.Message));
                if (ex.InnerException != null)
                {
                    Console.WriteLine();
                    Console.WriteLine(string.Format("InnerException Message: {0}", ex.InnerException.Message));
                }
            }
        }

        private static DbContextOptionsBuilder<OAuthServerContext> GetConnectionOptions(IConfiguration configuration)
        {
            string dbType = configuration["General:DbType"];
            string connectionString = configuration["ConnectionStrings:OAuthServerContextConnection"];
            string assemblyName = "Maraki1982.Web";

            var optionsBuilder = new DbContextOptionsBuilder<OAuthServerContext>();

            switch (dbType)
            {
                case "SQLite":
                    optionsBuilder.UseSqlite(connectionString, b => b.MigrationsAssembly(assemblyName));
                    break;
                case "SQLServer":
                    optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly(assemblyName));
                    break;
                case "PostgreSQL":
                    optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly(assemblyName));
                    break;
                default:
                    break;
            }

            return optionsBuilder;
        }
    }
}
