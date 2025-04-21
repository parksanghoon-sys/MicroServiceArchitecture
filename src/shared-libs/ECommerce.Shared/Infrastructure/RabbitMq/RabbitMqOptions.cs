namespace ECommerce.Shared.Infrastructure.RabbitMq;

public class RabbitMqOptions
{
    public const string RabbitMqSectionName = "RabbitMq";
    public const string RABBITMQ_DEFAULT_USER = "user";
    public const string RABBITMQ_DEFAULT_PASS = "user";

    public string HostName { get; set; } = string.Empty;
}
