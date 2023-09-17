using Microsoft.EntityFrameworkCore;
using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories;

public class ApplicationContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("ecommerce");
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();
}