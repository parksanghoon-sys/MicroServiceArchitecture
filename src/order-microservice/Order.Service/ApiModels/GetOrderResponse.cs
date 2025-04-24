namespace Order.Service.ApiModels;

public record GetOrderResponse(string CustomerId, Guid OrderId, DateTime OrderDate, List<GetOrderProductResponse> OrderProducts);
