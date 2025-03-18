using System.Xml.Serialization;

namespace Order.Service.Models;

/// <summary>
/// 외부 공개하지 않는 Order 제품 모델 클래스
/// </summary>
/// <param name="ProductId"></param>
/// <param name="Quantity"></param>
internal record OrderProduct(string ProductId, int Quantity)
{
    public int Quantity { get; private set; } = Quantity;

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }
}
