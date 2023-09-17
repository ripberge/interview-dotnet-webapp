using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public interface IProductRepository
{
    Product GetProduct(int? productId);
}

public class InMemoryProductRepository : IProductRepository
{
    private ApplicationContext _db;

    public InMemoryProductRepository(ApplicationContext db)
    {
        _db = db;
        Seed();
    }
    
    private void Seed()
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
    
    public Product GetProduct(int? productId)
    {
        return _db.Products.Single(p => p.Id == productId);
    }
}