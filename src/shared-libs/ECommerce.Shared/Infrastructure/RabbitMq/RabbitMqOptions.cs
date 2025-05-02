namespace ECommerce.Shared.Infrastructure.RabbitMq;

public class RabbitMqOptions
{
    public const string RabbitMqSectionName = "RabbitMq";
    public const string RABBITMQ_DEFAULT_USER = "guest";
    public const string RABBITMQ_DEFAULT_PASS = "guest";

    public string HostName { get; set; } = string.Empty;
}
