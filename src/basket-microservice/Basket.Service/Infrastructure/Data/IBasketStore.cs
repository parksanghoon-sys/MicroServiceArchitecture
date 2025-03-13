using Basket.Service.Models;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

namespace Basket.Service.Infrastructure.Data
{
    internal interface IBasketStore
    {
        CustomerBasket GetBasketByCustomerId(string customerId);
        void CreateCustomerBasket(CustomerBasket customerBasket);
        void UpdateCustomerBasket(CustomerBasket customerBasket);
        void DeleteCustomerBasket(string customerId);
    }
}
