using Auth.Service.Endpoints;
using Auth.Service.Infrastructure.Data.EntityFramework;
using Auth.Service.Services;
using Auth.Service.Services.IService;
using ECommerce.Shared.Authentication;
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSqlServerDatastore(builder.Configuration);

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MigrateDatabase();
    //app.ApplyOutboxMigrations();
}

app.RegisterEndpoints();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options
    .WithTheme(ScalarTheme.DeepSpace)
    .WithDarkModeToggle(true)
    .WithClientButton(true);
});
app.UseJwtAuthentication();

app.Run();
