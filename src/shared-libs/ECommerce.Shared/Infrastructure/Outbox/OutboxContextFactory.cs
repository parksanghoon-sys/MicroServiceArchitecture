using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.Shared.Infrastructure.Outbox;

internal class OutboxContextFactory : IDesignTimeDbContextFactory<OutboxContext>
{
    public OutboxContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<OutboxContext>();
        optionBuilder.UseNpgsql();

        return new OutboxContext(optionBuilder.Options);
    }
}
