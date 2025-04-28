using ECommerce.Shared.Infrastructure.EventBus;
using ECommerce.Shared.Infrastructure.RabbitMq;
using RabbitMQ.Client;
using Order.Service.Infrastructure.Data.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Order.Tests;

public class IntegrationTestBase : IClassFixture<OrderWebApplicationFactory>, IDisposable
{
    private const string QueueName = "order-integration-tests";
    private const string ExchangeName = "ecommerce-exchange";

    private IModel? _model;
    internal readonly OrderContext OrderContext;
    internal readonly HttpClient HttpClient;
    internal readonly IRabbitMqConnection RabbitMqConnection;
    internal List<Event> ReceivedEvents = [];

    public IntegrationTestBase(OrderWebApplicationFactory webApplicationFactory)
    {
        var scope = webApplicationFactory.Services.CreateScope();
        OrderContext = scope.ServiceProvider.GetRequiredService<OrderContext>();
        HttpClient = webApplicationFactory.CreateClient();
        RabbitMqConnection = scope.ServiceProvider.GetRequiredService<IRabbitMqConnection>();
    }
    public void Subscribe<TEVent>() where TEVent : Event
    {
        // RabbitMQ 연결에서 새 채널을 생성합니다.  
        _model = RabbitMqConnection.Connection.CreateModel();

        // fanout 타입의 교환기를 선언합니다. durable, autoDelete는 false로 설정합니다.  
        _model.ExchangeDeclare(ExchangeName, type: "fanout", durable: false, autoDelete: false, arguments: null);

        // 큐를 선언합니다. durable, exclusive, autoDelete는 false로 설정합니다.  
        _model.QueueDeclare(QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        // 이벤트를 수신하기 위한 소비자를 생성합니다.  
        EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(_model);

        // 이벤트 수신 시 호출될 핸들러를 정의합니다.  
        eventingBasicConsumer.Received += (sender, eventArgs) =>
        {
            // 메시지 본문을 UTF-8로 디코딩합니다.  
            var body = Encoding.UTF8.GetString(eventArgs.Body.Span);

            // 메시지를 지정된 이벤트 타입으로 역직렬화합니다.  
            var @event = JsonSerializer.Deserialize<TEVent>(body);

            // 역직렬화된 이벤트가 null이 아닌 경우 수신된 이벤트 리스트에 추가합니다.  
            if (@event is not null)
            {
                ReceivedEvents.Add(@event);
            }
        };

        // 큐에서 메시지를 소비하기 시작합니다. autoAck는 true로 설정합니다.  
        _model.BasicConsume(QueueName, autoAck: true, consumer: eventingBasicConsumer);

        // 큐를 교환기에 바인딩합니다. 라우팅 키는 이벤트 타입 이름으로 설정합니다.  
        _model.QueueBind(QueueName, ExchangeName, typeof(TEVent).Name);
    }
    /// <summary>
    /// RabbitMQ 를 정리
    /// </summary>
    public void Dispose()
    {
        if (_model is not null)
        {
            _model.QueueDelete(QueueName);
            _model.ExchangeDelete(ExchangeName);
        }
        RabbitMqConnection.Connection.Dispose();                
    }
}
