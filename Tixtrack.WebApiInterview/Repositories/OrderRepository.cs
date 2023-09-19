using Microsoft.EntityFrameworkCore;
using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public interface IOrderRepository
{
    Task<Order> Create(Order order);
    Task<IList<Order>> FindAll();
    Task<IList<Order>> FindActive();
    Task<Order?> FindById(int orderId);
    Task<Order> Save(Order order);
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

    public async Task<Order> Create(Order order)
    {
        await SaveAndDetach(entry: Db.Orders.Add(order));
        return order;
    }
    
    public async Task<IList<Order>> FindAll()
    {
        return await Db.Orders
            .Include(order => order.OrderProducts)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IList<Order>> FindActive()
    {
        return await Db.Orders
            .Include(order => order.OrderProducts)
            .Where(order => order.Status == OrderStatus.Active)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Order?> FindById(int orderId)
    {
        return Db.Orders
            .Include(order => order.OrderProducts)
            .AsNoTracking()
            .SingleOrDefaultAsync(order => order.Id == orderId);
    }

    public async Task<Order> Save(Order order)
    {
        await SaveAndDetach(entity: order);
        return order;
    }
}