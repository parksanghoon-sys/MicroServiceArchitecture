using ECommerce.Shared.Infrastructure.EventBus;
using ECommerce.Shared.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ECommerce.Shared.Infrastructure.Outbox;

public class OutboxBackgroundService : BackgroundService
{
    private readonly TimeSpan _period;
    private readonly IServiceProvider _serviceScopeFactory;
    private readonly ILogger<OutboxBackgroundService> _logger;
    public OutboxBackgroundService(IServiceProvider serviceProvider, 
                        IOptions<OutboxOptions> options, 
                        ILogger<OutboxBackgroundService> logger)
    {
        _serviceScopeFactory = serviceProvider;
        _logger = logger;
        _period = TimeSpan.FromSeconds(options.Value.PublishIntervalInSeconds);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new (_period);

        while (stoppingToken.IsCancellationRequested == false &&
                await timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Retrieving unpublished outbox events");

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var outboxStore = serviceScope.ServiceProvider.GetRequiredService<IOutboxStore>();
            var eventBus = serviceScope.ServiceProvider.GetRequiredService<IEventBus>();

            var unpublishedEvents = await outboxStore.GetUnpublishedOutboxEventsAsync();

            foreach(var unpublishedEvent in unpublishedEvents)
            {
                var @event = JsonSerializer.Deserialize(unpublishedEvent.Data, Type.GetType(unpublishedEvent.EventType)) as Event;
                
                await eventBus.PublishAsync(@event);

                await outboxStore.MarkOutboxEventAsPublishedAsync(unpublishedEvent.Id);
            }

            if (unpublishedEvents.Count == 0)
            {
                _logger.LogInformation("Unpublished outbox events sent");
            }
            else
            {
                _logger.LogInformation("No unpublished events to send");
            }
        }
    }
}
