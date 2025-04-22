using Basket.Service.ApiModels;
using Basket.Service.Infrastructure.Data;
using Basket.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Service.Endpoints;

public static class BsketApiEndpoints
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/{customerId}", ([FromServices]IBasketStore basketStore,string customerId)
            => basketStore.GetBasketByCustomerId(customerId));

        routeBuilder.MapPost("/{customerId}",
                            async ([FromServices] IBasketStore basketStore,
                                    [FromServices] IDistributedCache cache, 
                                    string customerId, 
                                    CreateBasketRequest createBasketRequest) =>
                              {
                                  var customerBasket = new CustomerBasket { CustomerId = customerId };

                                  var cachedProductPrice = decimal.Parse(await cache.GetStringAsync(createBasketRequest.ProductId));

                                  customerBasket.AddBasketProduct(new BasketProduct(createBasketRequest.ProductId, createBasketRequest.ProductName, cachedProductPrice));

                                  basketStore.CreateCustomerBasket(customerBasket);

                                  return TypedResults.Created();
                              });


        routeBuilder.MapPut("/{customerId}", async
                            ([FromServices] IBasketStore basketStore,
                            [FromServices] IDistributedCache cache, 
                            string customerId, 
                            AddBasketProductRequest addProductRequest) =>
       {
           var customerBasket = await basketStore.GetBasketByCustomerId(customerId);
           var cachedProductPrice = decimal.Parse(await cache.GetStringAsync(addProductRequest.ProductId));

           customerBasket.AddBasketProduct(new BasketProduct(addProductRequest.ProductId,
                                            addProductRequest.ProductName, cachedProductPrice, addProductRequest.Quantity));

           basketStore.UpdateCustomerBasket(customerBasket);

           return TypedResults.NoContent();
       });


        routeBuilder.MapDelete("/{customerId}/{productId}", async
            ([FromServices] IBasketStore basketStore, string customerId, string productId) =>
            {
                var customerBasket = await basketStore.GetBasketByCustomerId(customerId);
                
                customerBasket.RemoveBasketProduct(productId);

                await basketStore.UpdateCustomerBasket(customerBasket);

                return TypedResults.NoContent();
            });

        routeBuilder.MapDelete("/{customerId}",
            ([FromServices] IBasketStore basketStore, string customerId) =>
            {
                basketStore.DeleteCustomerBasket(customerId);

                return TypedResults.NoContent();
            });
    }
}