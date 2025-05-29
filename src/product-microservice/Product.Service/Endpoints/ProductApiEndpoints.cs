using ECommerce.Shared.Infrastructure.EventBus.Abstractions;
using ECommerce.Shared.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Product.Service.ApiModels;
using Product.Service.Infrastructure.Data;
using Product.Service.IntegrationEvents;
using System.Transactions;

namespace Product.Service.Endpoints
{
    public static class ProductApiEndpoints
    {
        public static void RegisterEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/productTypes", GetProductTypes);

            routeBuilder.MapGet("/{productId}", GetProductIdIProductStore);

            routeBuilder.MapPost("/", AddProduct);

            routeBuilder.MapPut("/{productId}", UpdateProduct);
        }
        internal static async Task<IResult> GetProductTypes(IProductStore productStore)
        {
            var producttypes = await productStore.GatProducTypeAll();

            return producttypes is null
                    ? TypedResults.NotFound("Product not found")
                    : TypedResults.Ok(new GetProductTypesResponse(producttypes));
        }

        internal static async Task<IResult> GetProductIdIProductStore(IProductStore productStore,
            int productId)
        {
            var product = await productStore.GetById(productId);

            return product is null
                    ? TypedResults.NotFound("Product not found")
                    : TypedResults.Ok(new GetProductResponse(product.Id, product.Name, product.Price, product.ProductType.Type, product.Description));
        }

        internal static async Task<IResult> AddProduct(IProductStore productStore, CreateProductRequest request)
        {
            var product = new Models.Product
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                ProductTypeId = request.ProductTypeId
            };

            await productStore.CreateProduct(product);

            return TypedResults.Created(product.Id.ToString());
        }

        internal static async Task<IResult> UpdateProduct(IProductStore productStore,            
            IOutboxStore outboxStore,
            int productId,
            UpdateProductRequest request)
        {
            var product = await productStore.GetById(productId);

            if (product is null)
            {
                return TypedResults.NotFound($"Product with id {productId} does not exist");
            }

            var existingPrice = product.Price;

            product.Name = request.Name;
            product.Price = request.Price;
            product.ProductTypeId = request.ProductTypeId;
            product.Description = request.Description;

            await outboxStore.CreateExecutionStrategy().ExecuteAsync(async () =>
           {
               using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

               await productStore.UpdateProduct(product);

               if (!decimal.Equals(existingPrice, request.Price))
               {
                   await outboxStore.AddOutboxEventAsync(new ProductPriceUpdatedEvent(productId, request.Price));
               }

               scope.Complete();
           });


            return TypedResults.NoContent();
        }

    }
}
