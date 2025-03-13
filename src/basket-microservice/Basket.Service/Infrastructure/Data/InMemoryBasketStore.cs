using Basket.Service.Models;

namespace Basket.Service.Infrastructure.Data
{
    internal class InMemoryBasketStore : IBasketStore
    {
        private static readonly Dictionary<string, CustomerBasket> _baskets = [];
        public CustomerBasket GetBasketByCustomerId(string customerId) => _baskets.TryGetValue(customerId, out var value) ? value : new CustomerBasket { CustomerId = customerId };
        public void CreateCustomerBasket(CustomerBasket customerBasket) => _baskets[customerBasket.CustomerId] = customerBasket;

        public void UpdateCustomerBasket(CustomerBasket customerBasket) 
        {
            if (_baskets.TryGetValue(customerBasket.CustomerId, out _))
            {
                _baskets[customerBasket.CustomerId] = customerBasket;
            }
            else
                CreateCustomerBasket(customerBasket);
        }

        public void DeleteCustomerBasket(string customerId)
        {
            _baskets.Remove(customerId);
        }
        
    }
}
