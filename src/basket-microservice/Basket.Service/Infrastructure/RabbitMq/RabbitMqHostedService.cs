using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Basket.Service.Infrastructure.Data;
using Basket.Service.IntegrationEvents;

namespace Basket.Service.Infrastructure.RabbitMq;

public class RabbitMqHostedService : IHostedService
{
    private const string ExchangeName = "ecommerce-exchange";
    private const string QueueName = "basket-microservice";

    private readonly IServiceProvider _serviceProvider;

    public RabbitMqHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Factory.StartNew(() =>
        {
            var rabbitMQConnection = _serviceProvider.GetRequiredService<IRabbitMqConnection>();

            var channel = rabbitMQConnection.Connection.CreateModel();

            // 교환이 존재하는지 확인 메시드            
            channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: "fanout",
                durable: false,
                autoDelete: false,
                null);
            // 큐에 대한 특정 이벤트 이름을 사용 대신 필드 이름을 사용
            // Basic 설정
            channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += OnMessageReceived;

            channel.BasicConsume(
                queue: QueueName,
                autoAck: true,
                consumer: consumer);

            channel.QueueBind(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: string.Empty);
        },
        TaskCreationOptions.LongRunning);

        return Task.CompletedTask;
    }

    private void OnMessageReceived(object? sender, BasicDeliverEventArgs eventArgs)
    {
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        var @event = JsonSerializer.Deserialize(message, typeof(OrderCreatedEvent)) as OrderCreatedEvent;

        using var scope = _serviceProvider.CreateScope();

        var basketStore = scope.ServiceProvider.GetRequiredService<IBasketStore>();

        basketStore.DeleteCustomerBasket(@event.CustomerId);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
