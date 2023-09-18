﻿using Moq;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Services;
using Xunit;

namespace TixTrack.WebApiInterview.UnitTests.Services;

public partial class SalesReportServiceImplTests
{
    [Fact]
    public async Task ProductSaleIsNotDifferentFromPriceTimesQuantity()
    {
        var expectedProduct = _firstValidProduct with { Price = 10 };
        var expectedQuantity = 2;
        _mockFindById(returnValue: expectedProduct);

        var actualSales = await _salesReportService.GetProductSales(new OrderProduct
        {
            ProductId = expectedProduct.Id,
            Quantity = expectedQuantity
        });
        
        var expectedSales = expectedProduct.Price * expectedQuantity;
        Assert.Equal(expectedSales, actualSales);
    }

    [Fact]
    public async Task OrderSalesDoNotExcludeFirstProductSale()
    {
        var expectedProduct = _firstValidProduct;
        var expectedOrder = _firstValidOrder with
        {
            OrderProducts = new List<OrderProduct>
            {
                new() { ProductId = expectedProduct.Id, Quantity = 2}
            }
        };
        _mockFindById(expectedProduct);
        
        var expectedSales = await _salesReportService.GetProductSales(new OrderProduct
        {
            ProductId = expectedProduct.Id,
            Quantity = expectedOrder.OrderProducts.Single().Quantity
        });
        var actualSales = await _salesReportService.GetOrderSales(expectedOrder);
        
        Assert.Equal(expectedSales, actualSales);
    }
    
    [Fact]
    public async Task OrderSalesDoNotExcludeAnyProductSale()
    {
        var orderProducts = new List<OrderProduct>
        {
            new() { ProductId = _firstValidProduct.Id, Quantity = 2 },
            new() { ProductId = _secondValidProduct.Id, Quantity = 2 }
        };
        var expectedSales = (await Task.WhenAll(
            orderProducts.Select(_salesReportService.GetProductSales))).Sum();
        
        var actualSales = await _salesReportService.GetOrderSales(
            _firstValidOrder with { OrderProducts = orderProducts });
        
        Assert.Equal(expectedSales, actualSales);
    }

    [Fact]
    public async Task CancelledOrdersAreNotIncluded()
    {
        var (activeOrder, cancelledOrder) = (_firstValidOrder, _validCancelledOrder);
        _mockGetAllOrders(returnValue: new List<Order> { activeOrder, cancelledOrder });
        _mockGetActiveOrders(returnValue: new List<Order> { activeOrder });

        var actualSalesReport = await _salesReportService.GetAllTime();

        var expectedOrderCount = 1;
        var expectedOrderSales = await _salesReportService.GetOrderSales(activeOrder);
        Assert.Equal(expectedOrderCount, actualSalesReport.OrderCount);
        Assert.Equal(expectedOrderSales, actualSalesReport.TotalSales);
    }
}

public partial class SalesReportServiceImplTests
{
    private Order _firstValidOrder => new()
    {
        Id = 101,
        Status = OrderStatus.Active,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        OrderProducts = new List<OrderProduct> { new() { ProductId = 1, Quantity = 1 } }
    };
    private Order _secondValidOrder => new()
    {
        Id = 102,
        Status = OrderStatus.Active,
        Created = new DateTimeOffset(new DateTime(2023, 01, 02)),
        OrderProducts = new List<OrderProduct>
        {
            new() { ProductId = 1, Quantity = 1 },
            new() { ProductId = 2, Quantity = 5 }
        }
    };
    private Order _validCancelledOrder => new()
    {
        Id = 201,
        Status = OrderStatus.Cancelled,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        OrderProducts = new List<OrderProduct> { new() { ProductId = 1, Quantity = 3 } }
    };
    private Product _firstValidProduct => new()
    {
        Id = 1,
        Name = "T-shirt",
        AvailableQuantity = 100,
        Price = 10.50,
        Type = "Clothing"
    };
    private Product _secondValidProduct => new()
    {
        Id = 2,
        Name = "Souvenir Mug",
        AvailableQuantity = 1500,
        Price = 7.25,
        Type = "Souvenir",
    };
    
    private Mock<IOrderRepository> _orderRepositoryMock { get; set; }
    private Mock<IProductRepository> _productRepositoryMock { get; set; }
    private SalesReportServiceImpl _salesReportService { get; set; }

    public SalesReportServiceImplTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _salesReportService = new SalesReportServiceImpl(
            orderRepository: _orderRepositoryMock.Object,
            productRepository: _productRepositoryMock.Object);

        _mockFindById(returnValue: _firstValidProduct);
        _mockFindById(returnValue: _secondValidProduct);
        _mockGetAllOrders(
            returnValue: new List<Order> { _firstValidOrder, _secondValidOrder });
    }

    private void _mockFindById(Product returnValue)
    {
        _productRepositoryMock
            .Setup(it => it.FindById(It.Is<int>(id => id == returnValue.Id)))
            .Returns(Task.FromResult((Product?)returnValue));
    }

    private void _mockGetAllOrders(List<Order> returnValue)
    {
        _orderRepositoryMock
            .Setup(it => it.GetAllOrders())
            .Returns(Task.FromResult((IList<Order>)returnValue));
    }
    
    private void _mockGetActiveOrders(List<Order> returnValue)
    {
        _orderRepositoryMock
            .Setup(it => it.GetActiveOrders())
            .Returns(Task.FromResult((IList<Order>)returnValue));
    }
}