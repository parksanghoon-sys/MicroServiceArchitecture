
using Basket.Service.Endpoints;
using Basket.Service.Infrastructure.Data;
using Basket.Service.Infrastructure.Data.Redis;
using Basket.Service.IntegrationEvents;
using Basket.Service.IntegrationEvents.EventHandlers;
using ECommerce.Shared.Infrastructure.EventBus;
using ECommerce.Shared.Infrastructure.RabbitMq;
using ECommerce.Shared.Observability;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Version = "9.9";
        document.Info.Title = "Demo .NET 9 API";
        document.Info.Description = "This API demonstrates OpenAPI customization in a .NET 9 project.";
        document.Info.TermsOfService = new Uri("https://codewithmukesh.com/terms");
        document.Info.Contact = new OpenApiContact
        {
            //Name = "Mukesh Murugan",
            //Email = "mukesh@codewithmukesh.com",
            //Url = new Uri("https://codewithmukesh.com")
        };
        document.Info.License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddOpenTelemetryTracing("Basket", builder.Configuration, (traceBuilder) => traceBuilder.WithSqlInstrumentation());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRedisCache(builder.Configuration);

//builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<IBasketStore, InMemoryBasketStore>();
builder.Services.AddScoped<IBasketStore, RedisBasketStore>();

builder.Services.AddRabbitMqEventBus(builder.Configuration)
                .AddRabbitMqSubscriberService(builder.Configuration)                
                .AddEventHandler<OrderCreatedEvent, OrderCreatedEventHandler>()
                .AddEventHandler<ProductPriceUpdatedEvent, ProductPriceUpdatedEventHandler>();

builder.Services.AddHostedService<RabbitMqHostedService>();


var app = builder.Build();

app.RegisterEndpoints();
app.MapOpenApi();
//app.UseSwagger();
//app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Swagger"));

app.MapScalarApiReference(options =>
{
    options
    .WithTheme(ScalarTheme.DeepSpace)
    .WithDarkModeToggle(true)
    .WithClientButton(true);
});

app.UseHttpsRedirection();

app.Run();
