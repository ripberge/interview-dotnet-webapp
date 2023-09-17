using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public class OrderRepository
{
    private static ApplicationContext _db;

    public static void Seed(ApplicationContext db)
    {
        db.Orders.Add(new Order
        {
            Id = 101,
            Created = new DateTimeOffset(new DateTime(2023, 01, 01)),
            Product1Id = 1,
            Product1Quantiity = 1
        });
        db.Orders.Add(new Order
        {
            Id = 102,
            Created = new DateTimeOffset(new DateTime(2023, 01, 02)),
            Product1Id = 1,
            Product1Quantiity = 2,
            Product2Id = 3,
            Product2Quantiity = 5,
        });
        db.Orders.Add(new Order
        {
            Id = 103,
            Created = new DateTimeOffset(new DateTime(2023, 01, 03)),
            Product1Id = 2,
            Product1Quantiity = 2,
        });
        db.SaveChanges();
        
        _db = db;
    }
    
    public static IList<Order> GetAllOrders()
    {
        return _db.Orders.ToList();
    }

    public static Order CreateOrder(Order order)
    {
        _db.Orders.Add(order);
        _db.SaveChanges();
        return order;
    }
}