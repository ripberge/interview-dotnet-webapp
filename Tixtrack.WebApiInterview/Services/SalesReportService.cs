using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Services;

public interface ISalesReportService {
    Task<ReadSalesReportResponse> Compute(ReadSalesReportRequest request);
    Task<IList<ReadTopProductResponse>> GetTopTenProducts();
}

public class SalesReportServiceImpl : ISalesReportService
{
    private IOrderRepository _orderRepository { get; set; }
    private IProductRepository _productRepository { get; set; }
 
    public SalesReportServiceImpl(
        IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<ReadSalesReportResponse> Compute(ReadSalesReportRequest request)
    {
        return (await _getOrdersSales(request)).Aggregate(
            seed: new ReadSalesReportResponse(),
            func: (salesReport, orderSales) =>
            {
                salesReport.TotalSales += orderSales;
                salesReport.OrderCount++;
                return salesReport;
            });
    }

    private async Task<double[]> _getOrdersSales(ReadSalesReportRequest request)
    {
        var orders = await (request switch
        {
            ({ } oldestDate, { } newestDate) => _orderRepository.FindActiveWithCreatedDateBetweenDates(oldestDate, newestDate),
            ({ } oldestDate, null) => _orderRepository.FindActiveWithCreatedDateGreaterThan(oldestDate),
            (null, { } newestDate) => _orderRepository.FindActiveWithCreatedDateLessThan(newestDate),
            _ => _orderRepository.FindActive()
        });
        return await Task.WhenAll(orders.Select(GetOrderSales));
    }

    public async Task<double> GetOrderSales(Order order) =>
        (await Task.WhenAll(order.OrderProducts.Select(GetProductSales))).Sum();

    public async Task<double> GetProductSales(OrderProduct orderProduct)
    {
        var product = await _productRepository.FindById(orderProduct.ProductId!);
        return product!.Price * orderProduct.Quantity;
    }

    public async Task<IList<ReadTopProductResponse>> GetTopTenProducts()
    {
        var topProducts = await _orderRepository.FindTopOrderProductsByQuantity(count: 10);
        return (await Task.WhenAll(topProducts.Select(_getOrderProductDetails))).ToList();
    }

    private async Task<ReadTopProductResponse> _getOrderProductDetails(
        OrderProduct orderProduct)
    {
        var product = await _productRepository.FindById(orderProduct.ProductId!);
        return new ReadTopProductResponse(product!.Name, orderProduct.Quantity);
    }
}