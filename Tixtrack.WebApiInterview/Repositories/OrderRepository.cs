using Microsoft.EntityFrameworkCore;
using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateOrder(Order order);
    Task<IList<Order>> GetAllOrders();
    Task<IList<Order>> GetActiveOrders();
    Task<Order> SaveOrder(Order order);
}

public class InMemoryOrderRepository : InMemoryRepository, IOrderRepository
{
    public InMemoryOrderRepository(ApplicationContext db) : base(db)
    {
    }
    
    public override void Seed()
    {
        _seedOrders();
        _seedOrderProducts();
    }

    private void _seedOrders()
    {
        BulkInsertSync(Db.Orders, new List<Order>
        { 
            new() { Id = 101, Status = OrderStatus.Active, Created = Date(2023, 01, 01) },
            new() { Id = 102, Status = OrderStatus.Active, Created = Date(2023, 01, 02) },
            new() { Id = 103, Status = OrderStatus.Active, Created = Date(2023, 01, 03) }
        });

        DateTimeOffset Date(int year, int month, int day) =>
            new DateTimeOffset(new DateTime(year, month, day));
    }
    
    private void _seedOrderProducts()
    {
        BulkInsertSync(Db.OrderProducts, new List<OrderProduct>
        {
            new() { OrderId = 101, ProductId = 1, Quantity = 1 },
            new() { OrderId = 102, ProductId = 1, Quantity = 2 },
            new() { OrderId = 102, ProductId = 3, Quantity = 5 },
            new() { OrderId = 103, ProductId = 2, Quantity = 2 }
        });
    }

    public async Task<Order> CreateOrder(Order order)
    {
        await SaveAndDetach(entry: Db.Orders.Add(order));
        return order;
    }
    
    public async Task<IList<Order>> GetAllOrders()
    {
        return await Db.Orders
            .Include(order => order.OrderProducts)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IList<Order>> GetActiveOrders()
    {
        return await Db.Orders
            .Include(order => order.OrderProducts)
            .Where(order => order.Status == OrderStatus.Active)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order> SaveOrder(Order order)
    {
        await SaveAndDetach(entity: order);
        return order;
    }
}