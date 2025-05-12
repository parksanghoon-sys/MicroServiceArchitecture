namespace ECommerce.Shared.Infrastructure.RabbitMq;

public class RabbitMqOptions
{
    public const string RabbitMqSectionName = "RabbitMq";

    public string HostName { get; set; } = string.Empty;
    public int Port { get; set; } = 5672;
    public string? UserName { get; set; } = "guest";
    public string? Password { get; set; } = "guest";
    public string? VirtualHost { get; set; } = "/";
    public int RetryCount { get; set; } = 5;
    public int RetryDelayMs { get; set; } = 5000;
    public TimeSpan? RequestedConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
