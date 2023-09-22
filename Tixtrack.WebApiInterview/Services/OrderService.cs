using TixTrack.WebApiInterview.Dtos;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Exceptions;
using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Repositories.Context;

namespace TixTrack.WebApiInterview.Services;

public interface IOrderService
{
    Task<string> Create(CreateOrderDto orderDto);
    Task<IList<Order>> GetAll();
    Task<Order?> GetById(string orderId);
    Task Cancel(string orderId);
}

public class OrderServiceImpl : IOrderService
{
    private IOrderRepository _orderRepository { get; set; }
    private CreateOrderUseCase _createOrderUseCase { get; set; }
    private CancelOrderUseCase _cancelOrderUseCase { get; set; }

    public OrderServiceImpl(
        IOrderRepository orderRepository,
        CreateOrderUseCase createOrderUseCase,
        CancelOrderUseCase cancelOrderUseCase)
    {
        _orderRepository = orderRepository;
        _createOrderUseCase = createOrderUseCase;
        _cancelOrderUseCase = cancelOrderUseCase;
    }

    public Task<string> Create(CreateOrderDto orderDto) =>
        _createOrderUseCase.Execute(orderDto);
    
    public Task<IList<Order>> GetAll() => _orderRepository.FindAll();

    public Task<Order?> GetById(string orderId) => _orderRepository.FindById(orderId);

    public Task Cancel(string orderId) => _cancelOrderUseCase.Execute(orderId);
}

public class CreateOrderUseCase
{
    private ILogger<CreateOrderUseCase> _logger { get; set; }
    private IProductRepository _productRepository { get; set; }
    private IOrderRepository _orderRepository { get; set; }
    private IApplicationContext _db { get; set; }

    public CreateOrderUseCase(
        ILogger<CreateOrderUseCase> logger,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IApplicationContext db)
    {
        _logger = logger;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _db = db;
    }
    
#pragma warning disable CS8618
    public CreateOrderUseCase()
    {
    }
#pragma warning restore CS8618
    
    public async Task<string> Execute(CreateOrderDto orderDto)
    {
        return await _db.UseTransaction(async (commit, rollback) =>
        {
            var orderId = await _processCreation(orderDto);
            await commit();
            _logger.LogInformation("Created order with ID {Id}.", orderId);
            return orderId;
        });
    }

    private async Task<string> _processCreation(CreateOrderDto orderDto)
    {
        await _validateCanCreateOrder(orderDto);
        var order = await _orderRepository.Create(new Order
        {
            Status = OrderStatus.Active,
            Created = DateTimeOffset.Now,
            OrderProducts = orderDto.OrderProducts.Select(orderProduct => new OrderProduct
            {
                ProductId = orderProduct.ProductId,
                Quantity = orderProduct.Quantity
            }).ToList()
        });
        return order.Id;
    }

    private async Task _validateCanCreateOrder(CreateOrderDto orderDto)
    {
        _validateOrderHasProducts(orderDto.OrderProducts);
        await _validateProductsExist(orderDto.OrderProducts);
    }

    private void _validateOrderHasProducts(List<CreateOrderProductDto> productsDto)
    {
        if (!productsDto.Any())
            throw new InvalidProductQuantityException(message: "Order must have at least a single product.");
        if (!productsDto.All(orderProduct => orderProduct.Quantity > 0))
            throw new InvalidProductQuantityException(message: "Each order product must have a positive quantity.");
    }

    private async Task _validateProductsExist(List<CreateOrderProductDto> productsDto)
    {
        var products = await Task.WhenAll(productsDto
            .Select(productDto => productDto.ProductId)
            .Select(async id => (Id: id, Product: await _productRepository.FindById(id))));
        
        if (products.First(pair => pair.Product == null) is (string unknownId, null))
            throw new InvalidProductIdException($"Could not find existing product by ID {unknownId}.");
    }
}

public class CancelOrderUseCase
{
    private ILogger<CancelOrderUseCase> _logger { get; set; }
    private IOrderRepository _orderRepository { get; set; }
    private IProductRepository _productRepository { get; set; }
    private IApplicationContext _db { get; set; }

    public CancelOrderUseCase(
        ILogger<CancelOrderUseCase> logger,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IApplicationContext db)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _db = db;
    }

#pragma warning disable CS8618
    public CancelOrderUseCase()
    {
    }
#pragma warning restore CS8618

    public async Task Execute(string orderId)
    {
        await _db.UseTransaction(async (commit, rollback) =>
        {
            await _processCancellation(order: await _orderRepository.FindById(orderId));
            await commit();
            _logger.LogInformation("Cancelled order by ID {Id}.", orderId);
        });
    }

    private async Task _processCancellation(Order? order)
    {
        _validateCanCancelOrder(order);
        await _setOrderStatusCancelled(order!);
        await _cancelOrderProducts(order!);
    }

    private void _validateCanCancelOrder(Order? order)
    {
        if (order == null)
            throw new OrderNotFoundException(message: "Order could not be found.");
        if (order.Status != OrderStatus.Active)
            throw new OrderIsNotActiveException(message: "Order may have been already cancelled.");
    }

    private async Task _setOrderStatusCancelled(Order order)
    {
        order.Status = OrderStatus.Cancelled;
        await _orderRepository.Save(order);
    }

    private async Task _cancelOrderProducts(Order order)
    {
        foreach (var orderProduct in order.OrderProducts)
            await _cancelOrderProduct(orderProduct);
    }

    private async Task _cancelOrderProduct(OrderProduct orderProduct)
    {
        var product = await _productRepository.FindById(orderProduct.ProductId);
        product!.AvailableQuantity += orderProduct.Quantity;
        await _productRepository.Save(product);
    }
}