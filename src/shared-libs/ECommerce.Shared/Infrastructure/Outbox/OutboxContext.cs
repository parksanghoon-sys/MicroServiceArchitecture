using ECommerce.Shared.Infrastructure.EventBus;
using ECommerce.Shared.Infrastructure.Outbox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;

namespace ECommerce.Shared.Infrastructure.Outbox;

internal class OutboxContext : DbContext, IOutboxStore
{
    public OutboxContext(DbContextOptions<OutboxContext> options)
        :base(options)
    {
        
    }
    public DbSet<OutboxEvent> OutboxEvents{ get; set; }

    public async Task AddOutboxEventAsync<T>(T @event) where T : Event
    {
        var existingEvent = await OutboxEvents.FindAsync(@event.Id);

        if (existingEvent is not null) 
            return;

        await OutboxEvents.AddAsync(new OutboxEvent
        { 
            Id = @event.Id,
            EventType = @event.GetType().AssemblyQualifiedName,
            Data = JsonSerializer.Serialize(@event)
        });

        await SaveChangesAsync();
        
    }
    public Task<List<OutboxEvent>> GetUnpublishedOutboxEventsAsync()
    {
        return OutboxEvents.Where(o => !o.Sent).ToListAsync();
    }

    public async Task MarkOutboxEventAsPublishedAsync(Guid outboxEventId)
    {
        var outboxEvent = await OutboxEvents.FindAsync(outboxEventId);
        if(outboxEvent is not null)
        {
            outboxEvent.Sent = true;
            await SaveChangesAsync();
        }
    }
    public IExecutionStrategy CreateExecutionStrategy()
    {
        return Database.CreateExecutionStrategy();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxEventConfiguration());
    }
}