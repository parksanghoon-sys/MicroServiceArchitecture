using Microsoft.EntityFrameworkCore;

namespace Product.Service.Infrastructure.Data.EntityFramework
{
    public static class EntityFrameworkExtensions
    {
        public static void AddSqlServerDatastore(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<ProductContext>(options =>
                 options.UseNpgsql(configuration.GetConnectionString("Default"),
                 npgsqlOptionsAction: npgsqlOptions =>
                 {
                     npgsqlOptions.EnableRetryOnFailure(
                         maxRetryCount: 5,
                         maxRetryDelay: TimeSpan.FromSeconds(40),
                         errorCodesToAdd: null);
                 }));

            services.AddScoped<IProductStore, ProductContext>();
        }
    }
}
