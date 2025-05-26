using Auth.Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Auth.Service.Infrastructure.Data.EntityFramework
{
    public static class EntityFrameworkExtensions
    {
        public static void AddSqlServerDatastore(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<AuthContext>(options =>
                          options.UseNpgsql(configuration.GetConnectionString("Default"),
                          npgsqlOptionsAction: npgsqlOptions =>
                          {
                              npgsqlOptions.EnableRetryOnFailure(
                                  maxRetryCount: 5,
                                  maxRetryDelay: TimeSpan.FromSeconds(40),
                                  errorCodesToAdd: null);
                          }));


            services.AddScoped<IAuthStore, AuthContext>();
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthContext>()
                        .AddDefaultTokenProviders();


        }
    }
}
