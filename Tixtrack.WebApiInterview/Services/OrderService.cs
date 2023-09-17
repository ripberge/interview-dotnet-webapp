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
    private ILogger<OrderServiceImpl> _logger { get; set; }
    private IOrderRepository _orderRepository { get; set; }
    private IProductRepository _productRepository { get; set; }

    public OrderServiceImpl(
        ILogger<OrderServiceImpl> logger,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }
    
    public Task<IList<Order>> GetAll() => _orderRepository.GetAllOrders();

    public async Task<Order?> GetById(int orderId)
    {
        var orders = await _orderRepository.GetAllOrders();
        return orders.SingleOrDefault(order => order.Id == orderId);
    }
    
    public async Task<int> Create(Order order)
    {
        var orderId = (await _orderRepository.CreateOrder(order)).Id;
        _logger.LogInformation("Created order with ID {Id}.", orderId);
        return orderId;
    }
}