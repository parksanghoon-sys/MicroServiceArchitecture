using ECommerce.Shared.Infrastructure.Outbox;
using ECommerce.Shared.Infrastructure.RabbitMq;
using ECommerce.Shared.Observability;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using Order.Service.Endpoints;
using Order.Service.Infrastructure.Data.EntityFramework;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Version = "9.9";
        document.Info.Title = "Demo .NET 9 API";
        document.Info.Description = "This API demonstrates OpenAPI customization in a .NET 9 project.";
        document.Info.Contact = new OpenApiContact
        {
            Name = "Park",
            Url = new Uri("https://github.com/parksanghoon-sys")
        };
        document.Info.License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        };
        return Task.CompletedTask;
    });
});
const string serviceName = "Order";
builder.Services.AddSqlServerDatastore(builder.Configuration);

builder.Services.AddRabbitMqEventBus(builder.Configuration)
    .AddRabbitMqEventPublisher();

builder.Services.AddOpenTelemetryTracing(serviceName, builder.Configuration, (traceBuilder) => 
            traceBuilder.WithSqlInstrumentation())
                        .AddOpenTelemetryMetrics(serviceName, builder.Services, (metricBuilder) =>
                        {
                            metricBuilder.AddView("products-per-order", new ExplicitBucketHistogramConfiguration { Boundaries = [1, 2, 5, 10] });
                        });

builder.Services.AddOutbox(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UsePrometheunExporter();

app.RegisterEndpoints();
app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options
    .WithTheme(ScalarTheme.DeepSpace)
    .WithDarkModeToggle(true)
    .WithClientButton(true);
});

if (app.Environment.IsDevelopment())
{
    app.MigrateDatabase();
    app.ApplyOutboxMigrations();
}

app.UseHttpsRedirection();

app.Run();

public partial class Program;