using Microsoft.Extensions.Logging;
using Moq;
using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Exceptions;
using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Services;
using Xunit;

namespace TixTrack.WebApiInterview.UnitTests.Services;

public partial class CancelOrderUseCaseTests
{
    [Fact]
    public async Task OrderCanNotBeCancelledTwice()
    {
        var expectedOrder = _validOrder;
        _mockFindOrderById(returnValue: expectedOrder);

        async Task CancelOrderTwice()
        {
            await _cancelOrderUseCase.Execute(expectedOrder.Id!);
            await _cancelOrderUseCase.Execute(expectedOrder.Id!);
        }

        await Assert.ThrowsAsync<OrderIsNotActiveException>(CancelOrderTwice);
    }

    [Fact]
    public async Task ActiveOrderCanBeCancelledOnce()
    {
        var expectedOrder = _validOrder;
        _mockFindOrderById(returnValue: expectedOrder);
        
        await _cancelOrderUseCase.Execute(expectedOrder.Id!);

        var expectedOrderStatus = OrderStatus.Cancelled;
        Assert.Equal(expectedOrderStatus, expectedOrder.Status);
    }
}

public partial class CancelOrderUseCaseTests
{
    private Order _validOrder => new()
    {
        Id = "01HANZZ3C3DJ6Z3NTF53SSDAYC",
        Status = OrderStatus.Active,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        OrderProducts = new List<OrderProduct>
        {
            new() { ProductId = "01HAP05RW9A0V5Z8NZ57A73JMY", Quantity = 1 }
        }
    };
    private Product _validProduct => new()
    {
        Id = "01HAP05RW9A0V5Z8NZ57A73JMY",
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
            db: new ApplicationContextMock());
        
        _mockFindProductById(returnValue: _validProduct);
    }

    private void _mockFindProductById(Product returnValue)
    {
        _productRepositoryMock
            .Setup(it => it.FindById(It.Is<string>(id => id == returnValue.Id)))
            .Returns(Task.FromResult((Product?)returnValue));
    }
    
    private void _mockFindOrderById(Order returnValue)
    {
        _orderRepositoryMock
            .Setup(it => it.FindById(It.Is<string>(id => id == returnValue.Id)))
            .Returns(Task.FromResult((Order?)returnValue));
    }
}

public partial class CreateOrderUseCaseTests
{
    [Fact]
    public async Task OrderProductsMustNotBeEmpty()
    {
        var emptyOrder = new CreateOrderRequest();

        Task CreateEmptyOrder() => _createOrderUseCase.Execute(emptyOrder);

        await Assert.ThrowsAsync<InvalidProductQuantityException>(CreateEmptyOrder);
    }

    [Fact]
    public async Task OrderProductQuantityMustBeNonNegative()
    {
        var invalidOrder = _getValidOrderWithCustomProductQuantity(-1);

        Task CreateInvalidOrder() => _createOrderUseCase.Execute(invalidOrder);

        await Assert.ThrowsAsync<InvalidProductQuantityException>(CreateInvalidOrder);
    }

    [Fact]
    public async Task OrderProductQuantityMustNotBeZero()
    {
        var invalidOrder = _getValidOrderWithCustomProductQuantity(0);
        
        Task CreateInvalidOrder() => _createOrderUseCase.Execute(invalidOrder);

        await Assert.ThrowsAsync<InvalidProductQuantityException>(CreateInvalidOrder);
    }

    [Fact]
    public async Task OrderDoesNotContainInvalidProductIds()
    {
        _mockFindProductById(returnValue: new Product { Id = _getNewProductId() });

        Task CreateOrderWithInvalidProductId()
        {
            return _createOrderUseCase.Execute(
                _getValidOrderWithCustomProductId(_getNewProductId()));
        }

        await Assert.ThrowsAsync<InvalidProductIdException>(
            CreateOrderWithInvalidProductId);
    }
}

public partial class CreateOrderUseCaseTests
{
    private Mock<IOrderRepository> _orderRepositoryMock { get; set; }
    private Mock<IProductRepository> _productRepositoryMock { get; set; }
    private CreateOrderUseCase _createOrderUseCase { get; set; }
    
    public CreateOrderUseCaseTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _createOrderUseCase = new CreateOrderUseCase(
            logger: new Mock<ILogger<CreateOrderUseCase>>().Object,
            orderRepository: _orderRepositoryMock.Object,
            productRepository: _productRepositoryMock.Object,
            db: new ApplicationContextMock());
    }
    
    private void _mockFindProductById(Product returnValue)
    {
        _productRepositoryMock
            .Setup(it => it.FindById(It.Is<string>(id => id == returnValue.Id)))
            .Returns(Task.FromResult((Product?)returnValue));
    }
    
    private CreateOrderRequest _getValidOrderWithCustomProductQuantity(int productQuantity)
    {
        return _getValidOrderWithCustomProduct(new OrderProductDto
        {
            ProductId = _getNewProductId(),
            Quantity = productQuantity
        });
    }
    
    private CreateOrderRequest _getValidOrderWithCustomProductId(string productId)
    {
        return _getValidOrderWithCustomProduct(new OrderProductDto
        {
            ProductId = productId,
            Quantity = 1
        });
    }
    
    private CreateOrderRequest _getValidOrderWithCustomProduct(OrderProductDto product)
    {
        return new CreateOrderRequest
        {
            OrderProducts = new List<OrderProductDto> { product }
        };
    }

    private string _getNewProductId() => Ulid.NewUlid().ToString();
}