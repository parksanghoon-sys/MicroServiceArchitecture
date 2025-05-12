using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

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
    int retryAttempts = 5;
    int retryDelayMs = 5000; // 5 seconds
    private void InitializeConnection()
    {
        int retryCount = _options.RetryCount;
        int retryDelayMs = _options.RetryDelayMs;

        Exception lastException = null;
        for (int attempt = 1; attempt <= retryCount; attempt++)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _options.HostName,
                    Port = _options.Port,
                    UserName = _options.UserName,
                    Password = _options.Password,
                    VirtualHost = _options.VirtualHost ?? "/",
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                    // 자동 복구 사용 설정
                    AutomaticRecoveryEnabled = true,
                    // 연결 끊김 후 재시도 간격
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                _connection = factory.CreateConnection();
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;

                if (attempt < retryCount)
                {
                    Thread.Sleep(retryDelayMs);
                }
            }
        }
        //throw new BrokerUnreachableException(lastException, "Failed to connect to RabbitMQ after multiple attempts");
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}