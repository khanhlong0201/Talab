using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace Talab
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Running...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in init");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureAppConfiguration((hostingContext, configs) =>
                {
                    var environment = hostingContext.HostingEnvironment;

                    configs.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    configs.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    if (environment.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(environment.ApplicationName));
                        if (appAssembly != null)
                            configs.AddUserSecrets(appAssembly, optional: true);
                    }
                    configs.AddEnvironmentVariables();
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();
                    logging.AddConsole();

                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}
