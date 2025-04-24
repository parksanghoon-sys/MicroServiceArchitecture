using Order.Service.Models;

namespace Order.Service.Infrastructure.Data;

//internal class InMemoryOrderStore : IOrderStore
//{
//    private readonly static Dictionary<string, Models.Order> Orders = [];
//    public Task CreateOreder(Models.Order order)
//    {
//        Orders[$"{order.CustomerId}-{ order.OrderId}"] = order;
//    }

//    public Task<Models.Order?> GetCustomerOrderById(string customerId, string orderId)
//    {
//        return Orders.TryGetValue($"{customerId}-{orderId}", out var order) ? order : null;
//    }
//}
