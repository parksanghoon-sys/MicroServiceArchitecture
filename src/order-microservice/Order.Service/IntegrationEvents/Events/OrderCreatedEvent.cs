namespace Order.Service.IntegrationEvents.Events;

public record OrderCreatedEvent(string CustomerId) : Event;
