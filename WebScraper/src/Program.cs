using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;

using System;
using System.IO;

namespace WebScraper
{
    public class Program
    {
        const string LOG_PATH = @"log.txt";//..\..\..\..\..\log.txt

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void Main(string[] args)
        {
            //Setting up logging as soon as possible
            var configuration = GetConfiguration(args);
            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Creating the host");

                var host = CreateHostBuilder(args, configuration).Build();

                Log.Information("Starting up the host");

                host.Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host start-up failed");
            }
            finally
            {
                //maybe not needed
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
        {
            var host = Host.CreateDefaultBuilder(args);

            host.UseSerilog();

            host.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

            host.ConfigureAppConfiguration(builder =>
            {
                builder.AddConfiguration(configuration);
            });

            return host;
        }

        private static IConfiguration GetConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            return builder.Build();
        }

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            const string logTemplate = "[{Timestamp:yyyy:MM:dd:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}";

            return new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", configuration.GetValue<string>("AppName"))
                .Enrich.FromLogContext()
                .WriteTo.File(
                    LOG_PATH,
                    outputTemplate: logTemplate,
                    rollingInterval: RollingInterval.Day
                )
                .WriteTo.Console(
                    outputTemplate: logTemplate,
                    restrictedToMinimumLevel: LogEventLevel.Error
                )
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
