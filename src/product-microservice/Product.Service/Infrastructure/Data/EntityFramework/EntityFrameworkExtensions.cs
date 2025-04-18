using Microsoft.EntityFrameworkCore;

namespace Product.Service.Infrastructure.Data.EntityFramework
{
    public static class EntityFrameworkExtensions
    {
        public static void AddSqlServerDatastore(this IServiceCollection service, IConfigurationManager configuration)
        {
            var dbConnectionString = configuration.GetConnectionString("Default");
            service.AddDbContext<ProductContext>(option =>
                option.UseNpgsql(dbConnectionString));

            service.AddScoped<IProductStore, ProductContext>();
        }
    }
}
