using Basket.Service.Models;

namespace Basket.Service.Infrastructure.Data;

internal record CustomerBasketCacheModel(IEnumerable<BasketProduct> Products);
