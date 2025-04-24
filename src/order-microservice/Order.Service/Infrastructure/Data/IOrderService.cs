﻿namespace Order.Service.Infrastructure.Data;

internal interface IOrderStore
{
    Task CreateOreder(Models.Order order);
    Task<Models.Order?> GetCustomerOrderById(string customerId, string orderId);
}
