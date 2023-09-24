using Microsoft.EntityFrameworkCore;
using TixTrack.WebApiInterview.Entities;
using TixTrack.WebApiInterview.Repositories.Context;

namespace TixTrack.WebApiInterview.Repositories;

public interface IProductRepository
{
    Task<string> Insert(Product product);
    Task<Product?> FindById(string id);
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
                Id = "01HAP05RW9A0V5Z8NZ57A73JMY",
                Name = "T-shirt",
                AvailableQuantity = 100,
                Price = 10.50,
                Type = "Clothing"
            },
            new()
            {
                Id = "01HAP09BED95ST5G88HTCC9G9Q",
                Name = "Souvenir Mug",
                AvailableQuantity = 1500,
                Price = 7.25,
                Type = "Souvenir",
            },
            new()
            {
                Id = "01HAP0ARYM63JGXGFB4R5Q1S28",
                Name = "Refrigerator Magnet",
                AvailableQuantity = 5000,
                Price = 0.99,
                Type = "Souvenir",
            },
            new()
            {
                Id = "01HB2RY5N2DBZWZEDMDQEPXPJ1",
                Name = "Eco-Friendly Water Bottle",
                AvailableQuantity = 200,
                Price = 14.99,
                Type = "Sustainable Goods",
            }
        });
    }

    public async Task<string> Insert(Product product)
    {
        await SaveAndDetach(entry: Db.Products.Add(product));
        return product.Id!;
    }
    
    public Task<Product?> FindById(string id) =>
        Db.Products.AsNoTracking().SingleOrDefaultAsync(product => product.Id == id);

    public async Task<Product> Save(Product product)
    {
        await SaveAndDetach(entity: product);
        return product;
    }
}