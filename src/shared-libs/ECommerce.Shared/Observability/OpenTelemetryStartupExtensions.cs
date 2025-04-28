using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Observability;

public static class OpenTelemetryStartupExtensions
{
    public static OpenTelemetryBuilder AddOpenTelemetryTracing(this IServiceCollection services, string serviceName)
    {
        return services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName))
                .WithTracing(builder =>
                {
                    builder.AddConsoleExporter()
                            .AddAspNetCoreInstrumentation();
                });
    }
    public static TracerProviderBuilder WithSqlInstrumentation(this TracerProviderBuilder builder) => builder.AddSqlClientInstrumentation();
}
