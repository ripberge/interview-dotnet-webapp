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

    public double GetOrderSales(Order order) => order.OrderProducts.Sum(GetProductSales);

    public double GetProductSales(OrderProduct orderProduct) =>
        _productRepository.FindById(orderProduct.ProductId)!.Price * orderProduct.Quantity;
}