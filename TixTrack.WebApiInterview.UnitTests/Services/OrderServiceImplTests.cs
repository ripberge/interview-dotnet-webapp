using Microsoft.Extensions.Logging;
using Moq;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Exceptions;
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
        _mockFindById(returnValue: expectedOrder);

        var actualOrder = await _orderService.GetById(expectedOrder.Id);
        
        Assert.Equal(expectedOrder.Id, actualOrder?.Id);
    }
    
    [Fact]
    public async Task OrderIdMatchesDatabaseStoredId()
    {
        var expectedOrder = _validOrderWithoutProducts with { Id = 1 };
        _mockCreateOrder(returnValue: expectedOrder);

        var actualOrderId = await _orderService.Create(expectedOrder);
        
        Assert.Equal(expectedOrder.Id, actualOrderId);
    }
}

public partial class OrderServiceImplTests
{
    private Order _validOrder => new()
    {
        Id = 101,
        Status = OrderStatus.Active,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        OrderProducts = new List<OrderProduct> { new() { ProductId = 1, Quantity = 1 } }
    };
    private Order _validOrderWithoutProducts => new()
    {
        Id = 201,
        Status = OrderStatus.Active,
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
            productRepository: _productRepositoryMock.Object,
            db: new ApplicationContext(),
            cancelOrderUseCase: new Mock<CancelOrderUseCase>().Object);

        _mockFindById(returnValue: _validProduct);
    }
    
    private void _mockFindById(Product returnValue)
    {
        _productRepositoryMock
            .Setup(it => it.FindById(It.Is<int>(id => id == returnValue.Id)))
            .Returns(Task.FromResult((Product?)returnValue));
    }
    
    private void _mockFindById(Order returnValue)
    {
        _orderRepositoryMock
            .Setup(it => it.FindById(It.Is<int>(id => id == returnValue.Id)))
            .Returns(Task.FromResult((Order?)returnValue));
    }

    private void _mockCreateOrder(Order returnValue)
    {
        _orderRepositoryMock
            .Setup(it => it.Create(It.IsAny<Order>()))
            .Returns(Task.FromResult(returnValue));
    }
}

public partial class CancelOrderUseCaseTests
{
    [Fact]
    public async Task OrderCanNotBeCancelledTwice()
    {
        var expectedOrder = _validOrder;
        _mockFindById(returnValue: expectedOrder);

        async Task CancelOrderTwice()
        {
            await _cancelOrderUseCase.Execute(expectedOrder!.Id);
            await _cancelOrderUseCase.Execute(expectedOrder!.Id);
        }

        await Assert.ThrowsAsync<OrderIsNotActiveException>(CancelOrderTwice);
    }

    [Fact]
    public async Task ActiveOrderCanBeCancelledOnce()
    {
        var expectedOrder = _validOrder;
        _mockFindById(returnValue: expectedOrder);
        
        await _cancelOrderUseCase.Execute(expectedOrder!.Id);

        var expectedOrderStatus = OrderStatus.Cancelled;
        Assert.Equal(expectedOrderStatus, expectedOrder.Status);
    }
}

public partial class CancelOrderUseCaseTests
{
    private Order _validOrder => new()
    {
        Id = 101,
        Status = OrderStatus.Active,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        OrderProducts = new List<OrderProduct> { new() { ProductId = 1, Quantity = 1 } }
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
    private CancelOrderUseCase _cancelOrderUseCase { get; set; }
    
    public CancelOrderUseCaseTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _cancelOrderUseCase = new CancelOrderUseCase(
            logger: new Mock<ILogger<CancelOrderUseCase>>().Object,
            orderRepository: _orderRepositoryMock.Object,
            productRepository: _productRepositoryMock.Object,
            db: new ApplicationContext());

        _mockFindById(returnValue: _validProduct);
    }
    
    private void _mockFindById(Product returnValue)
    {
        _productRepositoryMock
            .Setup(it => it.FindById(It.Is<int>(id => id == returnValue.Id)))
            .Returns(Task.FromResult((Product?)returnValue));
    }
    
    private void _mockFindById(Order returnValue)
    {
        _orderRepositoryMock
            .Setup(it => it.FindById(It.Is<int>(id => id == returnValue.Id)))
            .Returns(Task.FromResult((Order?)returnValue));
    }
}