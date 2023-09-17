using Microsoft.Extensions.Logging;
using Moq;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Services;
using Xunit;

namespace TixTrack.WebApiInterview.UnitTests.Services;

public partial class OrderServiceImplTests
{
    [Fact]
    public async Task OrderIsNotFoundWhenIdDoesNotExists()
    {
        var expectedOrder = _validOrder;

        var unknownId = expectedOrder.Id + 1;
        var actualOrder = await _orderService.GetById(unknownId);
        
        Assert.Null(actualOrder);
    }
    
    [Fact]
    public async Task OrderIsFoundWhenIdExists()
    {
        var expectedOrder = _validOrderWithoutProducts;

        var actualOrder = await _orderService.GetById(expectedOrder.Id);
        
        Assert.Equal(expectedOrder.Id, actualOrder?.Id);
    }
    
    [Fact]
    public async Task OrderIdMatchesDatabaseStoredId()
    {
        var expectedOrder = _validOrderWithoutProducts with { Id = 1 };
        _orderRepositoryMock
            .Setup(it => it.CreateOrder(It.IsAny<Order>()))
            .Returns(Task.FromResult(expectedOrder));

        var actualOrderId = await _orderService.Create(expectedOrder);
        
        Assert.Equal(expectedOrder.Id, actualOrderId);
    }
}

public partial class OrderServiceImplTests
{
    private Order _validOrder => new()
    {
        Id = 101,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        OrderProducts = new List<OrderProduct> { new() { ProductId = 1, Quantity = 1 } }
    };
    private Order _validOrderWithoutProducts => new()
    {
        Id = 201,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
    };
    private Product _validProduct => new()
    {
        Id = 1,
        Name = "T-shirt",
        AvailableQuantity = 100,
        Price = 10.50,
        Type = "Clothing"
    };
    
    private Mock<IOrderRepository> _orderRepositoryMock { get; set; }
    private Mock<IProductRepository> _productRepositoryMock { get; set; }
    private OrderServiceImpl _orderService { get; set; }

    public OrderServiceImplTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _orderService = new OrderServiceImpl(
            logger: new Mock<ILogger<OrderServiceImpl>>().Object,
            orderRepository: _orderRepositoryMock.Object,
            productRepository: _productRepositoryMock.Object);

        _orderRepositoryMock
            .Setup(it => it.GetAllOrders())
            .Returns(Task.FromResult((IList<Order>)new List<Order>
            {
                _validOrder,
                _validOrderWithoutProducts
            }));
        _productRepositoryMock
            .Setup(it => it.FindById(It.Is<int>(id => id == _validProduct.Id)))
            .Returns(Task.FromResult((Product?)_validProduct));
    }
}