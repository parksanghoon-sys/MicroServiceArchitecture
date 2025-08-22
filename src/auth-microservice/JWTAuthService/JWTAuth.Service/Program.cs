using Humanizer.DateTimeHumanizeStrategy;
using JWTAuth.Service.Context;
using JWTAuth.Service.Models;
using JWTAuth.Service.Services;
using JWTAuth.Service.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddJwtAuthentication(builder.Configuration);


builder.Services.AddSqlServerDatastore(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        //Seed Default Users
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await ApplicationDbContextSeed.SeedEssentialsAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseJwtAuthentication();

app.MapControllers();

app.Run();


public static class AuthServicekExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {        
        services.Configure<JWT>(configuration.GetSection("JWT"));

        services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddScoped<IUserService, UserService>();

    }
    public static void AddSqlServerDatastore(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
                      options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                      npgsqlOptionsAction: npgsqlOptions =>
                      {
                          npgsqlOptions.EnableRetryOnFailure(
                              maxRetryCount: 5,
                              maxRetryDelay: TimeSpan.FromSeconds(40),
                              errorCodesToAdd: null);
                      })
                      .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
                      .EnableDetailedErrors());


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // HTTPS 연결이 필요한지 여부
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
                };
            });

        services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

        //services.AddScoped<IAuthStore, AuthContext>();
    }
    public static void UseJwtAuthentication(this WebApplication app)
    {
        app.UseAuthentication().UseAuthorization();
    }
}