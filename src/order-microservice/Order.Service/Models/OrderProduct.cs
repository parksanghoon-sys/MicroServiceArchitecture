﻿using System.Xml.Serialization;

namespace Order.Service.Models;
/// <summary>
/// 외부 공개하지 않는 Order 제품 모델 클래스
/// </summary>
internal class OrderProduct
{
    public required string ProductId { get; init; }
    public int Quantity { get; private set; }
    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }
}

/// <summary>
/// Order 클래스
/// </summary>
internal class Order
{
    private List<OrderProduct> _orderProducts = [];
    public IReadOnlyCollection<OrderProduct> OrderProducts => _orderProducts.AsReadOnly();
    public required string CustomerId { get; init; }
    public Guid OrderId { get; init; }
    public DateTime OrderDate { get; private set; }

    public Order()
    {
        OrderId = Guid.NewGuid();
        OrderDate = DateTime.UtcNow;
    }
    public void AddOrderProduct(string productId, int quantity)
    {
        var existingOrderForProduct = _orderProducts.SingleOrDefault(o => o.ProductId == productId);
        if(existingOrderForProduct is not null)
        {
            existingOrderForProduct.AddQuantity(quantity);
        }
        else
        {
            var orderProduct = new OrderProduct { ProductId = productId };
            orderProduct.AddQuantity(quantity);

            _orderProducts.Add(orderProduct);
        }
    }
}
