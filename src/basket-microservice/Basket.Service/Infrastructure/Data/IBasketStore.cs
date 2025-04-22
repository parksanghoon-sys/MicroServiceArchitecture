using Basket.Service.Models;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

namespace Basket.Service.Infrastructure.Data
{
    internal interface IBasketStore
    {
        Task<CustomerBasket> GetBasketByCustomerId(string customerId);
        Task CreateCustomerBasket(CustomerBasket customerBasket);
        Task UpdateCustomerBasket(CustomerBasket customerBasket);
        Task DeleteCustomerBasket(string customerId);
    }
}
