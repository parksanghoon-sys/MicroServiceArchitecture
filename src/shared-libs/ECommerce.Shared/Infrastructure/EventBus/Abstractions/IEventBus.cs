namespace ECommerce.Shared.Infrastructure.EventBus.Abstractions;

interface IEventBus
{
    Task PublishAsync(Event @event);
}
