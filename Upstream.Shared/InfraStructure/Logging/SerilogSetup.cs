using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace Upstream.Shared.InfraStructure.Logging
{
    public class SerilogSetup
    {
        public static void ConfigureSerilog(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((ctx, services, loggerConfig) =>
            {
                loggerConfig
                    .ReadFrom.Configuration(ctx.Configuration)   // picks config from SerilogConfig.json
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", ApplicationConstants.ApplicationName)
                    .Enrich.WithProperty("Environment", ApplicationConstants.EnvironmentName)
                    .Enrich.WithProperty("OS", ApplicationConstants.OperatingSystem)
                    .Enrich.WithProperty("ProcessId", ApplicationConstants.ProcessId);
                    //.WriteTo.Console()
                    //.WriteTo.File(
                    //    path: Path.Combine(AppContext.BaseDirectory, "Logs", "shared-log-.txt"),
                    //    rollingInterval: RollingInterval.Day,
                    //    restrictedToMinimumLevel: LogEventLevel.Information
                    //);  // dont hardcode sinks as it can vary for each environment
                          // best practice is that give the sinks in configuration settings.
            });
        }
    }
}
