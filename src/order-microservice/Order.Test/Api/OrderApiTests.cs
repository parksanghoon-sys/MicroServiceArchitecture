using Order.Service.ApiModels;
using Order.Service.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Order.Tests.Api
{
    public class OrderApiTests : IntegrationTestBase
    {
        public OrderApiTests(OrderWebApplicationFactory orderWebApplicationFactory)
            :base(orderWebApplicationFactory) 
        {
            
        }
        [Fact]
        public async Task GetOrder_WhenNoOrderExists_ThenReturnsNotFound()
        {
            var response = await HttpClient.GetAsync($"/1/{Guid.NewGuid()}");
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task GetOrder_WhenOrderExists_ThenReturnsOrder()
        {
            var order = new Service.Models.Order { CustomerId = "1" };
            await OrderContext.CreateOreder(order);

            var response = await HttpClient.GetAsync($"/{order.CustomerId}/{order.OrderId}");

            response.EnsureSuccessStatusCode();

            var getOrderResponse = await response.Content.ReadFromJsonAsync<GetOrderResponse>();

            Assert.NotNull(getOrderResponse);
            Assert.Equal(order.OrderId, getOrderResponse.OrderId);
        }
        [Fact]
        public async Task CreateOrder_WhenCalled_ThenCreatesOrder()
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest([]);

            // Act
            var response = await HttpClient.PostAsJsonAsync("/1", createOrderRequest);

            // Assert
            response.EnsureSuccessStatusCode();

            var locationHeader = response.Headers.FirstOrDefault(h => string.Equals(h.Key, "Location")).Value.FirstOrDefault();

            Assert.NotNull(locationHeader);
            var split = locationHeader.Split('/');
            var customerId = split[0];
            var orderId = split[1];

            var order = OrderContext.Orders.FirstOrDefault(o => o.OrderId == Guid.Parse(orderId) && o.CustomerId == customerId);
            Assert.NotNull(order);
        }
        [Fact]
        public async Task CreateOrder_WhenCalled_ThenOrderCreatedEventPublished()
        {
            // Arrange
            const string customerId = "1";
            var createOrderRequest = new CreateOrderRequest([]);
            Subscribe<OrderCreatedEvent>();

            // Act
            var response = await HttpClient.PostAsJsonAsync($"/{customerId}", createOrderRequest);

            response.EnsureSuccessStatusCode();

            Assert.NotEmpty(ReceivedEvents);

            var receivedEvent = ReceivedEvents.First();

            Assert.IsType<OrderCreatedEvent>(receivedEvent);
            Assert.Equal((receivedEvent as OrderCreatedEvent).CustomerId, customerId);
        }
    }
}
