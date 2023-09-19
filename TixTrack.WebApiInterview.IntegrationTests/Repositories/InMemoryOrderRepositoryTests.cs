using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;
using Xunit;

namespace TixTrack.WebApiInterview.IntegrationTests.Repositories;

public partial class InMemoryOrderRepositoryTests
{
    [Fact]
    public async Task ActiveOrdersDoNotContainCancelledOrders()
    {
        var expectedOrders = new List<Order> { _activeOrder, _cancelledOrder };
        await Task.WhenAll(expectedOrders.Select(_orderRepository.Create));

        var actualActiveOrders = await _orderRepository.FindActive();

        Assert.Single(actualActiveOrders);
        Assert.Equal(expectedOrders.First(), actualActiveOrders.Single());
    }

    [Fact]
    public async Task AllOrdersContainActiveAndCancelledOrders()
    {
        var expectedOrders = new List<Order> { _activeOrder, _cancelledOrder };
        await Task.WhenAll(expectedOrders.Select(_orderRepository.Create));
        
        var actualOrders = await _orderRepository.FindAll();

        Assert.All(expectedOrders,
            expectedOrder => Assert.Contains(expectedOrder, actualOrders));
    }
}

public partial class InMemoryOrderRepositoryTests
{
    private Order _activeOrder => new()
    {
        Id = "01HAP11BKZN3XKZCVA1TJZTT4T",
        Status = OrderStatus.Active,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        OrderProducts = new List<OrderProduct>
        {
            new() { ProductId = "01HAP05RW9A0V5Z8NZ57A73JMY", Quantity = 1 }
        }
    };
    private Order _cancelledOrder => new()
    {
        Id = "01HAP11MBTH9ZNF9X21RMAY70C",
        Status = OrderStatus.Cancelled,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        OrderProducts = new List<OrderProduct>
        {
            new() { ProductId = "01HAP05RW9A0V5Z8NZ57A73JMY", Quantity = 1 }
        }
    };
    
    private InMemoryOrderRepository _orderRepository { get; set; }

    public InMemoryOrderRepositoryTests()
    {
        var db = new ApplicationContext();
        db.Database.EnsureDeleted();
        _orderRepository = new InMemoryOrderRepository(db);
    }
}