using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TixTrack.WebApiInterview.Repositories;

public abstract class InMemoryRepository
{
    protected ApplicationContext Db { get; set; }

    public InMemoryRepository(ApplicationContext db) => Db = db;

    public virtual void Seed()
    {
    }

    protected void BulkInsertSync<TEntity>(
        DbSet<TEntity> database, List<TEntity> entities) where TEntity : class
    {
        _saveAndDetachSync(entities.Select(database.Add).ToList());
    }

    private void _saveAndDetachSync<TEntity>(List<EntityEntry<TEntity>> entries)
        where TEntity : class
    {
        Db.SaveChanges();
        entries.ForEach(entry => entry.State = EntityState.Detached);
    }
    
    protected Task SaveAndDetach<TEntity>(TEntity entity) where TEntity : class =>
        SaveAndDetach(Attach(entity));

    protected EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class
    {
        var entry = Db.Entry(entity);
        entry.State = EntityState.Modified;
        return entry.State == EntityState.Detached ? Db.Attach(entity) : entry;
    }
    
    protected async Task SaveAndDetach<TEntity>(EntityEntry<TEntity> entry) where TEntity : class
    {
        await Db.SaveChangesAsync();
        Detach(entry);
    }

    protected void Detach<TEntity>(EntityEntry<TEntity> entry) where TEntity : class =>
        entry.State = EntityState.Detached;
}