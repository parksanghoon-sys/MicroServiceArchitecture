using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace ECommerce.Shared.Authentication;

public static class AuthenticationExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfigurationManager configuration)
    {
        var authOptions = new AuthenticationOptions();
        configuration.GetSection(AuthenticationOptions.AuthenticationSectionName).Bind(authOptions);
        services.AddSingleton(authOptions);
        
        
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidIssuer = authOptions.AuthMicroserviceBaseAddress,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.JwtOptions!.SecurityKey)),
                ValidAudience = authOptions.JwtOptions!.Audience
            };
        });

        services.AddAuthorization();
    }
    public static void UseJwtAuthentication(this WebApplication app) => app.UseAuthentication().UseAuthorization();
}
