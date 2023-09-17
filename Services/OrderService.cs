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
        IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }
    
    public IEnumerable<Order> GetAll()
    {
        var orders = _orderRepository.GetAllOrders();
        foreach (var o in orders)
        {
            if (o.Product1Id != null)
            {
                Product product = _productRepository.GetProduct(o.Product1Id);
                o.Product1Name = product.Name;
                o.Product1Price = product.Price;
            }
            if (o.Product2Id != null)
            {
                Product product = _productRepository.GetProduct(o.Product2Id);
                o.Product2Name = product.Name;
                o.Product2Price = product.Price;
            }
        }
        return orders;
    }
    
    public Order? GetById(int orderId)
    {
        var orders = _orderRepository.GetAllOrders();
        var order = orders.SingleOrDefault(o => o.Id == orderId);
        if (order != null)
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
        }
        return order;
    }
    
    public int Create(Order order)
    {
        _orderRepository.CreateOrder(order);
        return order.Id;
    }
}