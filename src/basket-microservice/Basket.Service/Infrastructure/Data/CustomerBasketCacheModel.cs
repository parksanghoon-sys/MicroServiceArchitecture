using Basket.Service.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Basket.Service.Infrastructure.Data;

internal record CustomerBasketCacheModel(IEnumerable<BasketProduct> Products);
