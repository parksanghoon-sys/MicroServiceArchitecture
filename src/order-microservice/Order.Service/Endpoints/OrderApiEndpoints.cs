using Order.Service.ApiModels;
using Order.Service.Infrastructure.Data;
using ECommerce.Shared.Infrastructure.EventBus.Abstractions;
using Order.Service.IntegrationEvents;
using ECommerce.Shared.Observability.Metrics;
using ECommerce.Shared.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Order.Service.Endpoints;

public static class OrderApiEndpoints
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/{customerId}", CreateOrder);
        routeBuilder.MapGet("/{customerId}/{orderId}", GetOrder);
    }
    internal static async Task<IResult> GetOrder(IOrderStore orderStore, string customerId, string orderId)
    {
        var order = await orderStore.GetCustomerOrderById(customerId, orderId);

        return order is null
            ? TypedResults.NotFound("Order not found for customer")
            : TypedResults.Ok(new GetOrderResponse
                                (order.CustomerId, order.OrderId, order.OrderDate,
                                order.OrderProducts.Select(op => new GetOrderProductResponse(op.ProductId, op.Quantity)).ToList()));
    }
    internal static async Task<IResult> CreateOrder(IEventBus eventBus,
        IOrderStore orderStore,
        MetricFactory metricFactory,
        IOutboxStore outboxStore,
        string customerId,
        CreateOrderRequest createOrderRequest)
    {
        var order = new Models.Order { CustomerId = customerId };

        await outboxStore.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            foreach (var product in createOrderRequest.OrderProducts)
            {
                order.AddOrderProduct(product.ProductId, product.Quantity);
                await orderStore.CreateOreder(order);
            }
            var orderCounter = metricFactory.Counter("total-orders", "Orders");
            orderCounter.Add(1);

            var productsPerOrderHistogram = metricFactory.Histogram("products-per-order", "Products");
            productsPerOrderHistogram.Record(order.OrderProducts.DistinctBy(p => p.ProductId).Count());

            await outboxStore.AddOutboxEventAsync(new OrderCreatedEvent(customerId));

            scope.Complete();
        });

        var orderCounter = metricFactory.Counter("total-orders", "Orders");
        orderCounter.Add(1);

        var productsPerOrderHistogram = metricFactory.Histogram("products-per-order", "Products");
        productsPerOrderHistogram.Record(order.OrderProducts.DistinctBy(p => p.ProductId).Count());

        await eventBus.PublishAsync(new OrderCreatedEvent(customerId));

        return TypedResults.Created($"{order.CustomerId}/{order.OrderId}");
    }
}
