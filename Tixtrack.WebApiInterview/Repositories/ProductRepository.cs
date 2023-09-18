using Microsoft.EntityFrameworkCore;
using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public interface IProductRepository
{
    Task<int> Insert(Product product);
    Task<Product?> FindById(int id);
    Task<Product> Save(Product product);
}

public class InMemoryProductRepository : InMemoryRepository, IProductRepository
{
    public InMemoryProductRepository(ApplicationContext db) : base(db)
    {
    }
    
    public override void Seed()
    {
        BulkInsertSync(Db.Products, new List<Product>
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

    public async Task<int> Insert(Product product)
    {
        await SaveAndDetach(entry: Db.Products.Add(product));
        return product.Id;
    }
    
    public Task<Product?> FindById(int id) =>
        Db.Products.AsNoTracking().SingleOrDefaultAsync(product => product.Id == id);

    public async Task<Product> Save(Product product)
    {
        await SaveAndDetach(entity: product);
        return product;
    }
}