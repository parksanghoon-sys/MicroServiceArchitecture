using Basket.Service.Models;

namespace Basket.Tests;

public class CustomerBasketTestsDomainModel
{

    [Fact]
    public void GivenAnEmptyCustomerBasket_WhenCallingAddBasketProducet_ThenProductAddedToBasket()
    {
        // Arrange
        var product = new BasketProduct("1", "Test Name", 9.99M);
        var customerBasket = new CustomerBasket { CustomerId = "1" };

        // Act
        customerBasket.AddBasketProduct(product);

        Assert.Contains(product, customerBasket.Products);
    }
}
