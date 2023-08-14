using Microsoft.EntityFrameworkCore;

namespace Tixtrack.WebApiInterview;

public static class Database
{
    private static ApplicationContext _db = new ApplicationContext();

    public static void Seed()
    {
        SeedProducts();
        SeedOrders();
    }

    public static void SeedProducts()
    {
        _db.Products.Add(new Product
        {
            Id = 1,
            Name = "T-shirt",
            AvailabileQuantity = 100,
            Price = 10.50,
            Type = "Clothing"
        });
        _db.Products.Add(new Product
        {
            Id = 2,
            Name = "Souvenir Mug",
            AvailabileQuantity = 1500,
            Price = 7.25,
            Type = "Souvenir",
        });
        _db.Products.Add(new Product
        {
            Id = 3,
            Name = "Refrigerator Magnet",
            AvailabileQuantity = 5000,
            Price = 0.99,
            Type = "Souvenir",
        });
        _db.SaveChanges();
    }

    public static void SeedOrders()
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

    public static Product GetProduct(int? productId)
    {
        return _db.Products.Single(p => p.Id == productId);
    }

    public class ApplicationContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("ecommerce");
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Product> Products => Set<Product>();
    }
}
