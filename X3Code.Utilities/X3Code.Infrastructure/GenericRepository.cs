using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using X3Code.Utils.Extensions;

namespace X3Code.Infrastructure;

/// <summary>
/// Provides basic database operations and encapsulates the DbContext
/// </summary>
/// <typeparam name="TEntity">The Entity type this repository is used</typeparam>
/// <typeparam name="TContext">The underlying DbContext, which can be used for the TEntity</typeparam>
public abstract class GenericRepository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : class, IEntity, new() where TContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{TContext}"/> class.
    /// </summary>
    /// <param name="context">The context used to access the database.</param>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <typeparam name="TEntity">The type of the entities in the repository.</typeparam>
    protected GenericRepository(TContext context)
    {
        DataBase = context;
        Entities = context.Set<TEntity>();
    }

    /// <summary>
    /// Direct access on the context
    /// </summary>
    protected TContext DataBase { get; }

    /// <summary>
    /// Direct access to the context TEntity
    /// </summary>
    protected DbSet<TEntity> Entities { get; }

    #region Entity Framework database operations

    /// <summary>
    /// Search an return the entity
    /// </summary>
    /// <param name="predicate">The expression which should be used to search for an entity</param>
    /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
    /// <returns>The searched Entity, if found</returns>
    public TEntity Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().SingleOrDefault(predicate);
        }

        return Entities.SingleOrDefault(predicate);
    }

    /// <summary>
    /// Returns all entities for TEntity
    /// </summary>
    /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
    /// <returns>The searched Entity, if found</returns>
    public IEnumerable<TEntity> GetAll(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().AsQueryable().ToList();
        }

        return Entities.AsQueryable().ToList();
    }

    /// <summary>
    /// Returns all entities matching the given predicate as IEnumerable
    /// </summary>
    /// <param name="predicate">The predicate for selecting entities</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <returns></returns>
    public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().Where(predicate).ToList();
        }

        return Entities.Where(predicate).ToList();
    }

    /// <summary>
    /// Searches for the corresponding entities that match the predicate
    /// </summary>
    /// <param name="predicate">The predicate for selecting entities</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <returns></returns>
    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().Where(predicate).AsQueryable();
        }

        return Entities.Where(predicate).AsQueryable();
    }

    /// <summary>
    /// Searches for the corresponding entities that match the predicate
    /// </summary>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <returns></returns>
    public IQueryable<TEntity> Query(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().AsQueryable();
        }

        return Entities.AsQueryable();
    }

    /// <summary>
    /// Adds the entity to the database
    /// </summary>
    /// <param name="entity">The entity that should be saved in database</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    public void Add(TEntity entity, bool asNoTracking = false)
    {
        Entities.Add(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Adds all entities to the database
    /// </summary>
    /// <param name="entities">Entities that should be saved into database</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
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

    /// <summary>
    /// Removes the given entity from database
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    public void Remove(TEntity entity, bool asNoTracking = false)
    {
        Entities.Remove(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Removes all given entities from database
    /// </summary>
    /// <param name="entities">The entities to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
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

    /// <summary>
    /// Updates the given entity from database
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    public void Update(TEntity entity, bool asNoTracking = false)
    {
        Entities.Update(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Updates all given entities from database
    /// </summary>
    /// <param name="entities">The entities to update</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
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

    /// <summary>
    /// Search an return the entity
    /// </summary>
    /// <param name="predicate">The expression which should be used to search for an entity</param>
    /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
    /// <returns>The searched Entity, if found</returns>
    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        return await Entities.SingleOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Returns all entities for TEntity
    /// </summary>
    /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
    /// <returns>The searched Entity, if found</returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().ToListAsync();
        }

        return await Entities.ToListAsync();
    }

    /// <summary>
    /// Returns all entities matching the given predicate as IEnumerable
    /// </summary>
    /// <param name="predicate">The predicate for selecting entities</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <returns></returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().Where(predicate).ToListAsync();
        }

        return await Entities.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Adds the entity to the database
    /// </summary>
    /// <param name="entity">The entity that should be saved in database</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    public async Task AddAsync(TEntity entity, bool asNoTracking = false)
    {
        await Entities.AddAsync(entity);
        await DataBase.SaveChangesAsync();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Adds all entities to the database
    /// </summary>
    /// <param name="entities">Entities that should be saved into database</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
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

    /// <summary>
    /// Removes the given entity from database
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    public async Task RemoveAsync(TEntity entity, bool asNoTracking = false)
    {
        Entities.Remove(entity);
        await DataBase.SaveChangesAsync();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Removes all given entities from database
    /// </summary>
    /// <param name="entities">The entities to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
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

    /// <summary>
    /// Updates the given entity from database
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    public async Task UpdateAsync(TEntity entity, bool asNoTracking = false)
    {
        Entities.Update(entity);
        await DataBase.SaveChangesAsync();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Updates all given entities from database
    /// </summary>
    /// <param name="entities">The entities to update</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
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

    #endregion

    #region Bulk Writing

    /// <summary>
    /// Performs a bulk write operation for the provided entities into the specified table.
    /// </summary>
    /// <param name="entities">Collection of entities to be written in the database table.</param>
    /// <param name="tableName">Name of the database table where the entities should be written.</param>
    /// <param name="timeOutInSeconds">Optional: Number of seconds for the operations to complete. If not set, default is used.</param>
    /// <returns>Returns the number of rows copied.</returns>
    public async Task<int> BulkWrite(IEnumerable<TEntity> entities, string tableName, int timeOutInSeconds = 0)
    {
        var asList = entities.ToList();
        if (asList.Count == 0)
        {
            return 0;
        }

        var asDataTable = asList.ToDataTable(tableName);
        var mappings = GetMapping(typeof(TEntity));
        
        using var bulkWriter = new SqlBulkCopy(DataBase.Database.GetConnectionString());

        if (timeOutInSeconds > 0)
        {
            bulkWriter.BulkCopyTimeout = timeOutInSeconds;   
        }
        bulkWriter.DestinationTableName = tableName;
        foreach (var mapping in mappings)
        {
            bulkWriter.ColumnMappings.Add(mapping);
        }

        await bulkWriter.WriteToServerAsync(asDataTable);

        return bulkWriter.RowsCopied;
    }

    /// <summary>
    /// Generates a list of columns mappings between the source data and the target table based on the properties of the provided object type.
    /// </summary>
    /// <param name="objectType">The type of the objects to generate mappings for.</param>
    /// <returns>Returns an IEnumerable of column mappings.</returns>
    private IEnumerable<SqlBulkCopyColumnMapping> GetMapping(Type objectType)
    {
        var result = new List<SqlBulkCopyColumnMapping>();
        
        if (objectType == null)
        {
            return result;
        }

        var properties = objectType.GetProperties();
        foreach (var property in properties)
        {
            if (!property.CanWrite) continue;
            result.Add(new SqlBulkCopyColumnMapping(property.Name, property.Name));
        }

        return result;
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