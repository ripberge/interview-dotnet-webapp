using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public class ProductRepository
{
    private static ApplicationContext _db;
    
    public static void Seed(ApplicationContext db)
    {
        db.Products.Add(new Product
        {
            Id = 1,
            Name = "T-shirt",
            AvailabileQuantity = 100,
            Price = 10.50,
            Type = "Clothing"
        });
        db.Products.Add(new Product
        {
            Id = 2,
            Name = "Souvenir Mug",
            AvailabileQuantity = 1500,
            Price = 7.25,
            Type = "Souvenir",
        });
        db.Products.Add(new Product
        {
            Id = 3,
            Name = "Refrigerator Magnet",
            AvailabileQuantity = 5000,
            Price = 0.99,
            Type = "Souvenir",
        });
        db.SaveChanges();

        _db = db;
    }
    
    public static Product GetProduct(int? productId)
    {
        return _db.Products.Single(p => p.Id == productId);
    }
}