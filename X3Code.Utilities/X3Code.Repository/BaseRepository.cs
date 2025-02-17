using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace X3Code.Repository;

[Experimental("InProgress")]
public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected BaseRepository(DbContext context)
    {
        DataBase = context;
        Entities = context.Set<TEntity>();
    }

    protected DbContext DataBase { get; }

    protected DbSet<TEntity> Entities { get; }

    #region Entity Framework database operations

    public TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().SingleOrDefault(predicate);
        }

        return Entities.SingleOrDefault(predicate);
    }

    public IEnumerable<TEntity> GetAll(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().AsQueryable().ToList();
        }

        return Entities.AsQueryable().ToList();
    }

    public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().Where(predicate).ToList();
        }

        return Entities.Where(predicate).ToList();
    }

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().Where(predicate).AsQueryable();
        }

        return Entities.Where(predicate).AsQueryable();
    }

    public IQueryable<TEntity> Query(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().AsQueryable();
        }

        return Entities.AsQueryable();
    }

    public void Add(TEntity entity, bool asNoTracking = false)
    {
        Entities.Add(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    public void AddAll(IEnumerable<TEntity> entities, bool asNoTracking = false)
    {
        var asList = entities.ToList();

        Entities.AddRange(asList);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(asList);
        }
    }

    public void Remove(TEntity entity, bool asNoTracking = false)
    {
        Entities.Remove(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    public void RemoveAll(IEnumerable<TEntity> entities, bool asNoTracking = false)
    {
        var asList = entities.ToList();

        Entities.RemoveRange(asList);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(asList.ToList());
        }
    }

    public void Update(TEntity entity, bool asNoTracking = false)
    {
        Entities.Update(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    public void UpdateAll(IEnumerable<TEntity> entities, bool asNoTracking = false)
    {
        var asList = entities.ToList();

        Entities.UpdateRange(asList);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(asList.ToList());
        }
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        return await Entities.SingleOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().ToListAsync();
        }

        return await Entities.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().Where(predicate).ToListAsync();
        }

        return await Entities.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(TEntity entity, bool asNoTracking = false)
    {
        await Entities.AddAsync(entity);
        await DataBase.SaveChangesAsync();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    public async Task AddAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false)
    {
        var asList = entities.ToList();

        await Entities.AddRangeAsync(asList);
        await DataBase.SaveChangesAsync();

        if (asNoTracking)
        {
            RemoveFromTracking(asList);
        }
    }

    public async Task RemoveAsync(TEntity entity, bool asNoTracking = false)
    {
        Entities.Remove(entity);
        await DataBase.SaveChangesAsync();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    public async Task RemoveAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false)
    {
        var asList = entities.ToList();

        Entities.RemoveRange(asList);
        await DataBase.SaveChangesAsync();

        if (asNoTracking)
        {
            RemoveFromTracking(asList);
        }
    }

    public async Task UpdateAsync(TEntity entity, bool asNoTracking = false)
    {
        Entities.Update(entity);
        await DataBase.SaveChangesAsync();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    public async Task UpdateAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false)
    {
        var asList = entities.ToList();

        Entities.UpdateRange(asList);
        await DataBase.SaveChangesAsync();

        if (asNoTracking)
        {
            RemoveFromTracking(asList.ToList());
        }
    }

    public async Task<int> BulkWrite(IEnumerable<TEntity> entities, string tableName, int timeOutInSeconds)
    {
        var entityList = entities.ToList();
        if (entityList.Count == 0)
        {
            return 0;
        }
    
        var batchSize = 1000; // EF Core doesn't support native bulk insert, so use a manual batch size
        var totalWritten = 0;
    
        for (var i = 0; i < entityList.Count; i += batchSize)
        {
            var batch = entityList.Skip(i).Take(batchSize).ToList();
    
            await Entities.AddRangeAsync(batch);
            totalWritten += batch.Count;
    
            await DataBase.SaveChangesAsync();
    
            // Detach to avoid tracking issues
            RemoveFromTracking(batch);
        }
    
        return totalWritten;
    }

    #endregion
    
    private void RemoveFromTracking(TEntity entity)
    {
        DataBase.Entry(entity).State = EntityState.Detached;
    }

    private void RemoveFromTracking(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            DataBase.Entry(entity).State = EntityState.Detached;
        }
    }
}