using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public interface IProductRepository
{
    Product? FindById(int id);
}

public class InMemoryProductRepository : IProductRepository
{
    private ApplicationContext _db;

    public InMemoryProductRepository(ApplicationContext db)
    {
        _db = db;
        _seed();
    }
    
    private void _seed()
    {
        _bulkInsert(new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "T-shirt",
                AvailableQuantity = 100,
                Price = 10.50,
                Type = "Clothing"
            },
            new()
            {
                Id = 2,
                Name = "Souvenir Mug",
                AvailableQuantity = 1500,
                Price = 7.25,
                Type = "Souvenir",
            },
            new()
            {
                Id = 3,
                Name = "Refrigerator Magnet",
                AvailableQuantity = 5000,
                Price = 0.99,
                Type = "Souvenir",
            }
        });
    }

    private IEnumerable<int> _bulkInsert(List<Product> products)
    {
        products.ForEach(product => _db.Products.Add(product));
        _db.SaveChanges();
        return products.Select(product => product.Id);
    }

    public int Insert(Product product)
    {
        _db.Products.Add(product);
        _db.SaveChanges();
        return product.Id;
    }
    
    public Product? FindById(int id) =>
        _db.Products.SingleOrDefault(product => product.Id == id);

    public void Delete(int id)
    {
        var product = new Product { Id = id };
        _db.Products.Attach(product);
        _db.Products.Remove(product);
        _db.SaveChanges();
    }
}