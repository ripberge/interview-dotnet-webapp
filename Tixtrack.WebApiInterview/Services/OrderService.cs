using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Exceptions;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Services;

public interface IOrderService
{
    Task<int> Create(Order order);
    Task<IList<Order>> GetAll();
    Task<Order?> GetById(int orderId);
    Task Cancel(int orderId);
}

public class OrderServiceImpl : IOrderService
{
    private ILogger<OrderServiceImpl> _logger { get; set; }
    private IOrderRepository _orderRepository { get; set; }
    private IProductRepository _productRepository { get; set; }
    private ApplicationContext _db { get; set; }

    public OrderServiceImpl(
        ILogger<OrderServiceImpl> logger,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ApplicationContext db)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _db = db;
    }
    
    public async Task<int> Create(Order order)
    {
        var orderId = (await _orderRepository.CreateOrder(order)).Id;
        _logger.LogInformation("Created order with ID {Id}.", orderId);
        return orderId;
    }
    
    public Task<IList<Order>> GetAll() => _orderRepository.GetAllOrders();

    public async Task<Order?> GetById(int orderId)
    {
        var orders = await _orderRepository.GetAllOrders();
        return orders.SingleOrDefault(order => order.Id == orderId);
    }

    public async Task Cancel(int orderId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();

        var order = await GetById(orderId);
        _validateOrderCanBeCancelled(order);
        
        await _setOrderStatusCancelled(order!);
        await Task.WhenAll(order!.OrderProducts.Select(_cancelOrderProduct));
        
        await transaction.CommitAsync();
        
        _logger.LogInformation("Cancelled order by ID {Id}.", orderId);
    }

    private void _validateOrderCanBeCancelled(Order? order)
    {
        if (order == null) throw new OrderNotFoundException();
        if (order.Status != OrderStatus.Active) throw new OrderIsNotActiveException();
    }

    private async Task _setOrderStatusCancelled(Order order)
    {
        order.Status = OrderStatus.Cancelled;
        await _orderRepository.SaveOrder(order);
    }

    private async Task _cancelOrderProduct(OrderProduct orderProduct)
    {
        var product = await _productRepository.FindById(orderProduct.ProductId);
        product!.AvailableQuantity += orderProduct.Quantity;
        await _productRepository.Save(product);
    }
}