using ECommerce.Shared.Infrastructure.RabbitMq;
using ECommerce.Shared.Observability;
using Microsoft.OpenApi.Models;
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

builder.Services.AddOpenTelemetryTracing("Order", (traceBuilder) => traceBuilder.WithSqlInstrumentation());

builder.Services.AddSqlServerDatastore(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRabbitMqEventBus(builder.Configuration)
    .AddRabbitMqEventPublisher();    

var app = builder.Build();

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
}

app.UseHttpsRedirection();

app.Run();

public partial class Program;