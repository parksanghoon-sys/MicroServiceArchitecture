using Microsoft.EntityFrameworkCore;

namespace Product.Service.Infrastructure.Data.EntityFramework
{
    public static class EntityFrameworkExtensions
    {
        public static void AddSqlServerDatastore(this IServiceCollection service, IConfigurationManager configuration)
        {
            service.AddDbContext<ProductContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("Default")));

            service.AddScoped<IProductStore, ProductContext>();
        }
    }
}
