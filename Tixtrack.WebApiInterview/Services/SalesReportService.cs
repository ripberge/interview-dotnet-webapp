using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Services;

public interface ISalesReportService {
    SalesReport GetAllTime();
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
    
    public SalesReport GetAllTime()
    {
        IList<Order> orders = _orderRepository.GetAllOrders();
        int orderCount = 0;
        double totalSales = 0;
        foreach (var o in orders)
        {
            orderCount++;
            if (o.Product1Id != null)
            {
                Product product = _productRepository.GetProduct(o.Product1Id);
                totalSales += (product.Price * o.Product1Quantiity!.Value);
            }
            if (o.Product2Id != null)
            {
                Product product = _productRepository.GetProduct(o.Product2Id);
                totalSales += (product.Price * o.Product2Quantiity!.Value);
            }
        }

        return new SalesReport
        {
            TotalSales = totalSales,
            OrderCount = orderCount,
        };
    }
}