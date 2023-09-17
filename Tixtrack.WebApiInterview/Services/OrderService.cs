using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Services;

public interface IOrderService
{
    IEnumerable<Order> GetAll();
    Order? GetById(int orderId);
    int Create(Order order);
}

public class OrderServiceImpl : IOrderService
{
    private IOrderRepository _orderRepository { get; set; }
    private IProductRepository _productRepository { get; set; }

    public OrderServiceImpl(
        IOrderRepository orderRepository, IProductRepository productRepository) =>
        (_orderRepository, _productRepository) = (orderRepository, productRepository);
    
    public IEnumerable<Order> GetAll() =>
        _orderRepository.GetAllOrders();
    
    public Order? GetById(int orderId) =>
        _orderRepository.GetAllOrders().SingleOrDefault(order => order.Id == orderId);
    
    public int Create(Order order)
    {
        _orderRepository.CreateOrder(order);
        return order.Id;
    }
}