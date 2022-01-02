using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace mass_transit
{
    public class Program
    {
        public static int Main(string[] args)
        {
           // CreateHostBuilder(args).Build().Run();
           // Log.Logger = new LoggerConfiguration()
           //.MinimumLevel.Debug()
           //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           //.Enrich.FromLogContext()
           //.WriteTo.Console()
           //.CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog((host, log) =>
            {
                if (host.HostingEnvironment.IsProduction())
                    log.MinimumLevel.Information();
                else
                    log.MinimumLevel.Debug();

                log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                log.MinimumLevel.Override("Quartz", LogEventLevel.Information);
                log.WriteTo.Console();
            })
            .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            ;
    }
}
