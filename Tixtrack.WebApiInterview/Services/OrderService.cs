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
        _orderRepository.GetAllOrders().Select(_mapProducts);
    
    public Order? GetById(int orderId)
    {
        return _orderRepository.GetAllOrders()
            .Where(order => order.Id == orderId)
            .Select(_mapProducts)
            .SingleOrDefault();
    }

    private Order _mapProducts(Order order)
    {
        if (order.Product1Id != null)
        {
            Product product = _productRepository.GetProduct(order.Product1Id);
            order.Product1Name = product.Name;
            order.Product1Price = product.Price;
        }
        if (order.Product2Id != null)
        {
            Product product = _productRepository.GetProduct(order.Product2Id);
            order.Product2Name = product.Name;
            order.Product2Price = product.Price;
        }

        return order;
    }
    
    public int Create(Order order)
    {
        _orderRepository.CreateOrder(order);
        return order.Id;
    }
}