using Moq;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Services;
using Xunit;

namespace TixTrack.WebApiInterview.UnitTests.Services;

public partial class OrderServiceImplTests
{
    [Fact]
    public void OrderIsNotFoundWhenIdDoesNotExists()
    {
        var expectedOrder = _validOrder;

        var unknownId = expectedOrder.Id + 1;
        var actualOrder = _orderService.GetById(unknownId);
        
        Assert.Null(actualOrder);
    }
    
    [Fact]
    public void OrderIsFoundWhenIdExists()
    {
        var expectedOrder = _validOrderWithoutProducts;

        var actualOrder = _orderService.GetById(expectedOrder.Id);
        
        Assert.Equal(expectedOrder.Id, actualOrder?.Id);
    }
    
    [Fact]
    public void OrderIdMatchesDatabaseStoredId()
    {
        var expectedOrder = _validOrderWithoutProducts with { Id = 1 };
        _orderRepositoryMock
            .Setup(it => it.CreateOrder(It.IsAny<Order>()))
            .Returns(expectedOrder);

        var actualOrderId = _orderService.Create(expectedOrder);
        
        Assert.Equal(expectedOrder.Id, actualOrderId);
    }
}

public partial class OrderServiceImplTests
{
    private Order _validOrder => new()
    {
        Id = 101,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        Product1Id = 1,
        Product1Quantiity = 1
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
            orderRepository: _orderRepositoryMock.Object,
            productRepository: _productRepositoryMock.Object);
        
        _orderRepositoryMock
            .Setup(it => it.GetAllOrders())
            .Returns(new List<Order> { _validOrder, _validOrderWithoutProducts });
        _productRepositoryMock
            .Setup(it => it.FindById(It.Is<int>(id => id == _validProduct.Id)))
            .Returns(_validProduct);
    }
}