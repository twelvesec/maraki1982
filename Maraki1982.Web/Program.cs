using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
#if RELEASE
using System.IO;
using System.Reflection;
#endif

namespace Maraki1982.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
#if RELEASE
                    var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    webBuilder.UseContentRoot(currentDirectory);
#endif
                    webBuilder.UseStartup<Startup>();
                });
    }
}
