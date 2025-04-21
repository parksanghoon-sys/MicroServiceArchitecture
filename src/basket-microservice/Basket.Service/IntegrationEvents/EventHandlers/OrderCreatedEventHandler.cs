using Basket.Service.Infrastructure.Data;
using ECommerce.Shared.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Service.IntegrationEvents.EventHandlers;

internal class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IBasketStore _basketStore;
    public OrderCreatedEventHandler(IBasketStore basketStore) => _basketStore = basketStore;
    public Task Handle(OrderCreatedEvent @event)
    {
        _basketStore.DeleteCustomerBasket(@event.CustomerId);
        return Task.CompletedTask;
    }
}
public class ProductPriceUpdatedEventHandler : IEventHandler<ProductPriceUpdatedEvent>
{
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _cacheEntryOptions;
    public ProductPriceUpdatedEventHandler(IDistributedCache cache)
    {
        _cache = cache;
        _cacheEntryOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(24)
        };
    }
    public async Task Handle(ProductPriceUpdatedEvent @event)
    {
        var exitstingProductPrice = await _cache.GetStringAsync(@event.ProductId.ToString());

        if(exitstingProductPrice is null || string.Equals(exitstingProductPrice, @event.NewPrice.ToString()))
        {
            await _cache.SetStringAsync(@event.ProductId.ToString(), @event.NewPrice.ToString(), _cacheEntryOptions);
        }
    }
}