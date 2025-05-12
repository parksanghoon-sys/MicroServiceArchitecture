using RabbitMQ.Client;

namespace ECommerce.Shared.Infrastructure.RabbitMq;

public class RabbitMqConnection : IDisposable, IRabbitMqConnection
{
    private IConnection? _connection;
    private readonly RabbitMqOptions _options;

    public IConnection Connection => _connection!;

    public RabbitMqConnection(RabbitMqOptions options)
    {
        _options = options;

        InitializeConnection();
    }

    private void InitializeConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            // 가상 호스트 설정 (필요한 경우)
            VirtualHost = _options.VirtualHost ?? "/",
            // 연결 제한 시간 설정 (선택 사항)
            RequestedConnectionTimeout = TimeSpan.FromSeconds(30)
        };

        _connection = factory.CreateConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}