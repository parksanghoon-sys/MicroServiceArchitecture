using Microsoft.EntityFrameworkCore;

namespace Order.Service.Infrastructure.Data.EntityFramework
{
    public static class EntityFrameworkExtensions
    {
        public static void AddSqlServerDatastore(this IServiceCollection service, IConfigurationManager configuration)
        {
            var dbConnectionString = configuration.GetConnectionString("Default");
            service.AddDbContext<OrderContext>(option =>
                option.UseNpgsql(dbConnectionString));

            service.AddScoped<IOrderStore, OrderContext>();
        }
    }
}
