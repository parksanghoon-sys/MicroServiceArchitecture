namespace ECommerce.Shared.Infrastructure.EventBus;

public record Event
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public Event()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }
   
}
