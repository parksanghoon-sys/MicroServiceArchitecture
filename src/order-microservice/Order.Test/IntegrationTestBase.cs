using ECommerce.Shared.Infrastructure.EventBus;
using ECommerce.Shared.Infrastructure.RabbitMq;
using RabbitMQ.Client;
using Order.Service.Infrastructure.Data.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Order.Tests;

public class IntegrationTestBase : IClassFixture<OrderWebApplicationFactory>
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
}
