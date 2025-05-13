using ECommerce.Shared.Infrastructure.EventBus;
using ECommerce.Shared.Infrastructure.EventBus.Abstractions;
using ECommerce.Shared.Observability;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace ECommerce.Shared.Infrastructure.RabbitMq;
/// <summary>
/// IHostedService는 .NET Core 및 .NET 5 이상의 애플리케이션에서 백그라운드 작업을 구현하기 위한 인터페이스입니다. 이 인터페이스는 애플리케이션 시작 시 실행되고 종료 시 정리되는 백그라운드 서비스를 만들 수 있게 해줍니다.
/// IHostedService의 주요 역할
//애플리케이션 수명 주기 관리:
//애플리케이션이 시작될 때 실행되는 StartAsync(CancellationToken) 메서드
//애플리케이션이 종료될 때 실행되는 StopAsync(CancellationToken) 메서드
//백그라운드 작업 실행:
//주기적인 작업 실행
//장기 실행 프로세스
//큐 처리
//메시지 구독 처리
/// </summary>
public class RabbitMqHostedService : IHostedService
{
    private const string ExchangeName = "ecommerce-exchange";

    private readonly IServiceProvider _serviceProvider;
    private readonly EventHandlerRegistration _handlerRegistrations;
    private readonly EventBusOptions _eventBusOptions;
    private readonly ActivitySource _activitySource;
    private readonly TextMapPropagator _propagator = Propagators.DefaultTextMapPropagator;

    public RabbitMqHostedService(IServiceProvider serviceProvider,
        IOptions<EventHandlerRegistration> handlerRegistrations,
        IOptions<EventBusOptions> eventBusOptions,
        RabbitMqTelemetry rabbitMqTelemetry)
    {
        _serviceProvider = serviceProvider;
        _handlerRegistrations = handlerRegistrations.Value;
        _eventBusOptions = eventBusOptions.Value;
        _activitySource = rabbitMqTelemetry.ActivitySource;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Factory.StartNew(() =>
        {
            var rabbitMQConnection = _serviceProvider.GetRequiredService<IRabbitMqConnection>();
            var channel = rabbitMQConnection.Connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: "fanout",
                durable: false,
                autoDelete: false,
                null);

            channel.QueueDeclare(
                queue: _eventBusOptions.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += OnMessageReceived;

            channel.BasicConsume(
              queue: _eventBusOptions.QueueName,
              autoAck: true,
              consumer: consumer,
              consumerTag: string.Empty,
              noLocal: false,
              exclusive: false,
              arguments: null);

            foreach (var (eventName, _) in _handlerRegistrations.EventTypes)
            {
                channel.QueueBind(
                    queue: _eventBusOptions.QueueName,
                    exchange: ExchangeName,
                    routingKey: eventName,
                    arguments: null);
            }
        },TaskCreationOptions.LongRunning);

        return Task.CompletedTask;
    }

    private void OnMessageReceived(object? sender, BasicDeliverEventArgs eventArgs)
    {
        var parentContext = _propagator.Extract(default, eventArgs.BasicProperties, (properties, key) =>
        {
            if(properties.Headers.TryGetValue(key, out var value))
            {
                var bytes = value as byte[];
                return [Encoding.UTF8.GetString(bytes)];
            }
            return [];
        });

        var activityName = $"{OpenTelemetryMessagingConventions.ReceiveOperation} {eventArgs.RoutingKey}";

        using var activity = _activitySource.StartActivity(activityName, ActivityKind.Client, parentContext.ActivityContext);

        SetActivityContext(activity, eventArgs.RoutingKey, OpenTelemetryMessagingConventions.ReceiveOperation);

        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        activity?.SetTag("message", message);

        using var scope = _serviceProvider.CreateScope();

        if (_handlerRegistrations.EventTypes.TryGetValue(eventName, out var eventType) == false)
            return;

        var @event = JsonSerializer.Deserialize(message, eventType) as Event;

        foreach(var handler in scope.ServiceProvider.GetKeyedServices<IEventHandler>(eventType))
            handler.Handle(@event);
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    private static void SetActivityContext(Activity? activity, string routingKey, string operation)
    {
        if(activity is not null)
        {
            activity.SetTag(OpenTelemetryMessagingConventions.System, "rabbitmq");
            activity.SetTag(OpenTelemetryMessagingConventions.OperationName, operation);
            activity.SetTag(OpenTelemetryMessagingConventions.DestinationName, routingKey);
        }
    }
}
