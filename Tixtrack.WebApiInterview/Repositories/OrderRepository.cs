using Microsoft.EntityFrameworkCore;
using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public interface IOrderRepository
{
    Task<IList<Order>> GetAllOrders();
    Task<Order> CreateOrder(Order order);
}

public class InMemoryOrderRepository : IOrderRepository
{
    private ApplicationContext _db;

    public InMemoryOrderRepository(ApplicationContext db)
    {
        _db = db;
        _seedOrders();
        _seedOrderProducts();
    }

    private void _seedOrders()
    {
        _db.Orders.Add(new Order
        {
            Id = 101,
            Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
        });
        _db.Orders.Add(new Order
        {
            Id = 102,
            Created = new DateTimeOffset(new DateTime(2023, 01, 02)),
        });
        _db.Orders.Add(new Order
        {
            Id = 103,
            Created = new DateTimeOffset(new DateTime(2023, 01, 03)),
        });
        _db.SaveChanges();
    }
    
    private void _seedOrderProducts()
    {
        _db.OrderProducts.Add(new OrderProduct
        {
            OrderId = 101,
            ProductId = 1,
            Quantity = 1
        });
        _db.OrderProducts.Add(new OrderProduct
        {
            OrderId = 102,
            ProductId = 1,
            Quantity = 2
        });
        _db.OrderProducts.Add(new OrderProduct
        {
            OrderId = 102,
            ProductId = 3,
            Quantity = 5
        });
        _db.OrderProducts.Add(new OrderProduct
        {
            OrderId = 103,
            ProductId = 2,
            Quantity = 2
        });
        _db.SaveChanges();
    }
    
    public async Task<IList<Order>> GetAllOrders() => await _db.Orders.ToListAsync();

    public async Task<Order> CreateOrder(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }
}