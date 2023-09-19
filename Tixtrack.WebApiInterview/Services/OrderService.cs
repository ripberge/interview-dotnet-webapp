using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Exceptions;
using TixTrack.WebApiInterview.Repositories;

namespace TixTrack.WebApiInterview.Services;

public interface IOrderService
{
    Task<int> Create(CreateOrderDto orderDto);
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
    private CancelOrderUseCase _cancelOrderUseCase { get; set; }

    public OrderServiceImpl(
        ILogger<OrderServiceImpl> logger,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ApplicationContext db,
        CancelOrderUseCase cancelOrderUseCase)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _db = db;
        _cancelOrderUseCase = cancelOrderUseCase;
    }
    
    public async Task<int> Create(CreateOrderDto orderDto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        var orderId = await _processCreation(orderDto);
        await transaction.CommitAsync();
        
        _logger.LogInformation("Created order with ID {Id}.", orderId);
        return orderId;
    }

    private async Task<int> _processCreation(CreateOrderDto orderDto)
    {
        var orderId = Random.Shared.Next();
        await _orderRepository.Create(new Order
        {
            Id = orderId,
            Status = OrderStatus.Active,
            Created = DateTimeOffset.Now,
            OrderProducts = orderDto.OrderProducts.Select(orderProduct => new OrderProduct
            {
                OrderId = orderId,
                ProductId = orderProduct.ProductId,
                Quantity = orderProduct.Quantity
            }).ToList()
        });
        return orderId;
    }
    
    public Task<IList<Order>> GetAll() => _orderRepository.FindAll();

    public Task<Order?> GetById(int orderId) => _orderRepository.FindById(orderId);

    public Task Cancel(int orderId) => _cancelOrderUseCase.Execute(orderId);
}

public class CancelOrderUseCase
{
    private ILogger<CancelOrderUseCase> _logger { get; set; }
    private IOrderRepository _orderRepository { get; set; }
    private IProductRepository _productRepository { get; set; }
    private ApplicationContext _db { get; set; }

    public CancelOrderUseCase(
        ILogger<CancelOrderUseCase> logger,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ApplicationContext db)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _db = db;
    }

    public CancelOrderUseCase()
    {
    }

    public async Task Execute(int orderId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        await _processCancellation(order: await _orderRepository.FindById(orderId));
        await transaction.CommitAsync();
        
        _logger.LogInformation("Cancelled order by ID {Id}.", orderId);
    }

    private async Task _processCancellation(Order? order)
    {
        _validateOrderCanBeCancelled(order);
        await _setOrderStatusCancelled(order!);
        await _cancelOrderProducts(order!);
    }

    private void _validateOrderCanBeCancelled(Order? order)
    {
        if (order == null) throw new OrderNotFoundException();
        if (order.Status != OrderStatus.Active) throw new OrderIsNotActiveException();
    }

    private async Task _setOrderStatusCancelled(Order order)
    {
        order.Status = OrderStatus.Cancelled;
        await _orderRepository.Save(order);
    }

    private async Task _cancelOrderProducts(Order order)
    {
        foreach (var orderProduct in order!.OrderProducts)
            await _cancelOrderProduct(orderProduct);
    }

    private async Task _cancelOrderProduct(OrderProduct orderProduct)
    {
        var product = await _productRepository.FindById(orderProduct.ProductId);
        product!.AvailableQuantity += orderProduct.Quantity;
        await _productRepository.Save(product);
    }
}