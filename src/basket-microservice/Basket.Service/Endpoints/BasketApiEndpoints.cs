using Basket.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Service.Endpoints;

public static class BsketApiEndpoints
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/{customerId}", ([FromServices]IBasketStore basketStore, string customerId)
            => basketStore.GetBasketByCustomerId(customerId));

        //routeBuilder.MapPost("/{customerId}",
        //                    ([FromServices] IBasketStore basketStore, string customerId, CreateBasketRequest createBasketRequest) =>
        //                      {
        //                          var customerBasket = new CustomerBasket { CustomerId = customerId };

        //                          customerBasket.AddBasketProduct(new BasketProduct(createBasketRequest.ProductId, createBasketRequest.ProductName));

        //                          basketStore.CreateCustomerBasket(customerBasket);

        //                          return TypedResults.Created();
        //                      });
    }
}