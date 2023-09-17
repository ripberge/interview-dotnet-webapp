using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public interface IOrderRepository
{
    IList<Order> GetAllOrders();
    Order CreateOrder(Order order);
}

public class InMemoryOrderRepository : IOrderRepository
{
    private ApplicationContext _db;

    public InMemoryOrderRepository(ApplicationContext db)
    {
        _db = db;
        Seed();
    }

    private void Seed()
    {
        _db.Orders.Add(new Order
        {
            Id = 101,
            Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
            Product1Id = 1,
            Product1Quantiity = 1
        });
        _db.Orders.Add(new Order
        {
            Id = 102,
            Created = new DateTimeOffset(new DateTime(2023, 01, 02)),
            Product1Id = 1,
            Product1Quantiity = 2,
            Product2Id = 3,
            Product2Quantiity = 5,
        });
        _db.Orders.Add(new Order
        {
            Id = 103,
            Created = new DateTimeOffset(new DateTime(2023, 01, 03)),
            Product1Id = 2,
            Product1Quantiity = 2,
        });
        _db.SaveChanges();
    }
    
    public IList<Order> GetAllOrders()
    {
        return _db.Orders.ToList();
    }

    public Order CreateOrder(Order order)
    {
        _db.Orders.Add(order);
        _db.SaveChanges();
        return order;
    }
}