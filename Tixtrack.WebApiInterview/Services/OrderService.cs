using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Services;

public interface IOrderService
{
    Task<IList<Order>> GetAll();
    Task<Order?> GetById(int orderId);
    Task<int> Create(Order order);
}

public class OrderServiceImpl : IOrderService
{
    private IOrderRepository _orderRepository { get; set; }
    private IProductRepository _productRepository { get; set; }

    public OrderServiceImpl(
        IOrderRepository orderRepository, IProductRepository productRepository) =>
        (_orderRepository, _productRepository) = (orderRepository, productRepository);
    
    public Task<IList<Order>> GetAll() => _orderRepository.GetAllOrders();

    public async Task<Order?> GetById(int orderId)
    {
        var orders = await _orderRepository.GetAllOrders();
        return orders.SingleOrDefault(order => order.Id == orderId);
    }
    
    public async Task<int> Create(Order order)
    {
        await _orderRepository.CreateOrder(order);
        return order.Id;
    }
}