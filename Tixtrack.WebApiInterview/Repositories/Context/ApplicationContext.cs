using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TixTrack.WebApiInterview.Entities;

namespace TixTrack.WebApiInterview.Repositories.Context;

public interface IApplicationContext
{
    
    Task UseTransaction(Func<Func<Task>, Func<Task>, Task> action);
    Task<T> UseTransaction<T>(Func<Func<Task>, Func<Task>, Task<T>> action);
}

public class ApplicationContext : DbContext, IApplicationContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("ecommerce")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .Property(it => it.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<UlidValueGenerator>();
        
        modelBuilder.Entity<OrderProduct>()
            .HasKey(it => new { it.OrderId, it.ProductId });
    }

    public async Task UseTransaction(Func<Func<Task>, Func<Task>, Task> action)
    {
        await using var transaction = await Database.BeginTransactionAsync();
        await action(() => transaction.CommitAsync(), () => transaction.RollbackAsync());
    }

    public async Task<T> UseTransaction<T>(Func<Func<Task>, Func<Task>, Task<T>> action)
    {
        await using var transaction = await Database.BeginTransactionAsync();
        return await action(
            () => transaction.CommitAsync(),
            () => transaction.RollbackAsync());
    }
}