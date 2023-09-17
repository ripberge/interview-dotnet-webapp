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
        return _orderRepository.GetAllOrders().Aggregate(
            seed: new SalesReport(),
            func: (salesReport, order) =>
            {
                salesReport.TotalSales += GetOrderSales(order);
                salesReport.OrderCount++;
                return salesReport;
            }
        );
    }

    public double GetOrderSales(Order order)
    {
        double sales = 0;
        
        if (order.Product1Id != null)
            sales += GetProductSales(order.Product1Id, order.Product1Quantiity);
            
        if (order.Product2Id != null)
            sales += GetProductSales(order.Product2Id, order.Product2Quantiity);

        return sales;
    }

    public double GetProductSales(int? productId, int? productQuantity)
    {
        return productId == null
            ? 0
            : (_productRepository.FindById(productId.Value)?.Price ?? 0)
              * (productQuantity ?? 0);
    }
}