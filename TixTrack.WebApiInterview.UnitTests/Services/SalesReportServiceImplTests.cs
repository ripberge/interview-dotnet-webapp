using Moq;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Services;
using Xunit;

namespace TixTrack.WebApiInterview.UnitTests.Services;

public partial class SalesReportServiceImplTests
{
    [Fact]
    public void ProductSaleIsNotDifferentFromPriceTimesQuantity()
    {
        var expectedProduct = _firstValidProduct with { Price = 10 };
        var expectedQuantity = 2;
        _productRepositoryMock
            .Setup(it => it.FindById(It.IsAny<int>()))
            .Returns(expectedProduct);

        var actualSales = _salesReportService.GetProductSales(
            productId: expectedProduct.Id,
            productQuantity: expectedQuantity);
        
        var expectedSales = expectedProduct.Price * expectedQuantity;
        Assert.Equal(expectedSales, actualSales);
    }

    [Fact]
    public void OrderSalesDoNotExcludeFirstProductSale()
    {
        var expectedProduct = _firstValidProduct;
        var expectedOrder = _firstValidOrder with
        {
            Product1Id = expectedProduct.Id,
            Product1Quantiity = 2,
            Product2Id = null,
        };
        _productRepositoryMock
            .Setup(it => it.FindById(It.IsAny<int>()))
            .Returns(expectedProduct);

        var expectedSales = _salesReportService.GetProductSales(
            expectedProduct.Id, expectedOrder.Product1Quantiity);
        var actualSales = _salesReportService.GetOrderSales(expectedOrder);
        
        Assert.Equal(expectedSales, actualSales);
    }
    
    [Fact]
    public void OrderSalesDoNotExcludeAnyProductSale()
    {
        var expectedOrder = _firstValidOrder with
        {
            Product1Id = _firstValidProduct.Id,
            Product1Quantiity = 2,
            Product2Id = _secondValidProduct.Id,
            Product2Quantiity = 3
        };
        var expectedSales = new List<(int? Id, int? Quantity)>
        {
            (expectedOrder.Product1Id, expectedOrder.Product1Quantiity),
            (expectedOrder.Product2Id, expectedOrder.Product2Quantiity)
        }.Sum(p => _salesReportService.GetProductSales(p.Id, p.Quantity));
        
        var actualSales = _salesReportService.GetOrderSales(expectedOrder);
        
        Assert.Equal(expectedSales, actualSales);
    }
    
    [Fact]
    public void GetAllTimeSalesReportAggregatesAllOrderSales()
    {
        var orders = new List<Order> { _firstValidOrder, _secondValidOrder };
        var expectedTotalSales = orders.Sum(_salesReportService.GetOrderSales);
        var expectedOrderCount = orders.Count;

        var actualSalesReport = _salesReportService.GetAllTime();

        Assert.Equal(expectedTotalSales, actualSalesReport.TotalSales);
        Assert.Equal(expectedOrderCount, actualSalesReport.OrderCount);
    }
}

public partial class SalesReportServiceImplTests
{
    private Order _firstValidOrder => new()
    {
        Id = 101,
        Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        Product1Id = 1,
        Product1Quantiity = 1
    };
    private Order _secondValidOrder => new()
    {
        Id = 102,
        Created = new DateTimeOffset(new DateTime(2023, 01, 02)),
        Product1Id = 1,
        Product1Quantiity = 2,
        Product2Id = 2,
        Product2Quantiity = 5,
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
        
        new List<Product> { _firstValidProduct, _secondValidProduct }.ForEach(product =>
        {
            _productRepositoryMock
                .Setup(it => it.FindById(It.Is<int>(id => id == product.Id)))
                .Returns(product);
        });
        _orderRepositoryMock
            .Setup(it => it.GetAllOrders())
            .Returns(new List<Order> { _firstValidOrder, _secondValidOrder });
    }
}