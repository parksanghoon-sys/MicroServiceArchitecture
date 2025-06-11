using Microsoft.AspNetCore.Builder;
using Serilog;


namespace ECommerce.Shared.Log
{
    public static class SerilogExtensions
    {
        public static void AddSerilogLog(this WebApplicationBuilder webApplication)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(webApplication.Configuration)
                .WriteTo.Console()
                .WriteTo.File(
                    path: "logs/app-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 31,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            webApplication.Host.UseSerilog(Serilog.Log.Logger);

        }
    }
}
