using Order.Service.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Order.Service.Infrastructure.Data;
using ECommerce.Shared.Infrastructure.EventBus.Abstractions;
using Order.Service.IntegrationEvents;

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
                                (order.CustomerId,order.OrderId, order.OrderDate, 
                                order.OrderProducts.Select(op => new GetOrderProductResponse(op.ProductId, op.Quantity)).ToList()));
    }
    internal static async Task<IResult> CreateOrder(IEventBus eventBus, IOrderStore orderStore, string customerId, CreateOrderRequest createOrderRequest)
    {
        var order = new Models.Order { CustomerId = customerId };
        foreach (var product in createOrderRequest.OrderProducts)
        {
            order.AddOrderProduct(product.ProductId, product.Quantity);
        }
        await orderStore.CreateOreder(order);

        await eventBus.PublishAsync(new OrderCreatedEvent(customerId));

        return TypedResults.Created($"{order.CustomerId}/{order.OrderId}");
    }
}
