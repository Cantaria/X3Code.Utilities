using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace X3Code.Repository;

/// <summary>
/// Represents a generic base repository for managing data access operations on entities.
/// Provides methods for CRUD operations, query execution, and bulk operations.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity to be managed by the repository. Must be a class.
/// </typeparam>
[Experimental("InProgress")]
public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Provides a base implementation for repository operations across entities of a specific type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that the repository will manage.</typeparam>
    public BaseRepository(DbContext context)
    {
        DataBase = context;
        Entities = context.Set<TEntity>();
    }

    /// <summary>
    /// Represents the database context associated with the repository.
    /// </summary>
    /// <remarks>
    /// This property provides access to the underlying <see cref="DbContext"/> used by the repository.
    /// It is intended for internal use within the repository to manage database operations.
    /// </remarks>
    protected DbContext DataBase { get; }

    /// <summary>
    /// Represents a DbSet instance for the specified entity type in the context.
    /// It provides access to query and manage TEntity records within the database.
    /// This property is used for performing CRUD and query operations on the database.
    /// </summary>
    protected DbSet<TEntity> Entities { get; }

    #region Entity Framework database operations

    /// <summary>
    /// Retrieves a single entity that matches the specified predicate.
    /// Optionally, the result can be retrieved without tracking changes.
    /// </summary>
    /// <param name="predicate">The condition to filter the entity.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether to retrieve the entity without tracking changes.
    /// Default is false.
    /// </param>
    /// <returns>
    /// The entity that matches the predicate if found, otherwise null.
    /// </returns>
    public TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().SingleOrDefault(predicate);
        }

        return Entities.SingleOrDefault(predicate);
    }

    /// Retrieves all entities of type TEntity from the database.
    /// <param name="asNoTracking">
    /// A boolean value indicating whether the entities should be retrieved without tracking.
    /// When true, the entities are not tracked by the DbContext, which is useful for read-only
    /// operations to improve performance.
    /// </param>
    /// <returns>
    /// A collection of entities of type TEntity.
    /// </returns>
    public IEnumerable<TEntity> GetAll(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().AsQueryable().ToList();
        }

        return Entities.AsQueryable().ToList();
    }

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="TEntity"/> that satisfy the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter criteria for retrieving entities.</param>
    /// <param name="asNoTracking">Specifies whether the query should be executed with tracking disabled. If set to true, changes to the returned entities will not be persisted to the database.</param>
    /// <returns>An enumerable collection of entities that match the specified predicate.</returns>
    public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().Where(predicate).ToList();
        }

        return Entities.Where(predicate).ToList();
    }

    /// <summary>
    /// Queries the database for entities that match the specified predicate or retrieves all entities.
    /// </summary>
    /// <param name="predicate">
    /// A lambda expression defining the filter criteria that the entities must satisfy.
    /// </param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether the query should be executed in "no tracking" mode,
    /// which avoids tracking changes in the retrieved entities for improved performance.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{TEntity}"/> representing the entities that satisfy the predicate or the entire dataset if no predicate is provided.
    /// </returns>
    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().Where(predicate).AsQueryable();
        }

        return Entities.Where(predicate).AsQueryable();
    }

    /// <summary>
    /// Retrieves a queryable collection of entities from the database.
    /// </summary>
    /// <param name="asNoTracking">
    /// A value indicating whether the entities should be tracked by the context.
    /// If set to true, the entities will not be tracked.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{TEntity}"/> representing the queryable collection of entities.
    /// </returns>
    public IQueryable<TEntity> Query(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().AsQueryable();
        }

        return Entities.AsQueryable();
    }

    /// <summary>
    /// Adds a new entity to the repository and saves the changes to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether the entity should be removed
    /// from tracking after being saved. Default is false.
    /// </param>
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
    /// Adds a collection of entities to the database and saves the changes.
    /// </summary>
    /// <param name="entities">Collection of entities to be added.</param>
    /// <param name="asNoTracking">
    /// If true, the entities will not be tracked by the DbContext after being added.
    /// </param>
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
    /// Removes the specified entity from the database and saves the changes. Optionally stops tracking the entity in the context.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <param name="asNoTracking">Indicates whether the entity should be removed from tracking after deletion.</param>
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
    /// Removes a collection of entities from the database context and persists the changes.
    /// </summary>
    /// <param name="entities">The collection of entities to be removed from the database.</param>
    /// <param name="asNoTracking">Indicates whether the entities should be removed from the tracking system after removal. Default is false.</param>
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
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether the entity should be removed from the tracking system.
    /// If set to true, the entity will not be tracked by the context after the update.
    /// </param>
    public void Update(TEntity entity, bool asNoTracking = false)
    {
        Entities.Update(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// Updates a collection of entities in the database.
    /// <param name="entities">The collection of entities to update in the database.</param>
    /// <param name="asNoTracking">Indicates whether the entities should be removed from tracking after being updated. Default is false.</param>
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
    /// Asynchronously retrieves a single entity that matches the specified predicate.
    /// </summary>
    /// <param name="predicate">A function that defines the condition to filter the entity.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether or not the entity should be tracked by the context.
    /// Set to true for better performance when only read access is required.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the entity
    /// that matches the predicate, or null if no such entity is found.
    /// </returns>
    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        return await Entities.SingleOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Asynchronously retrieves all entities of type <typeparamref name="TEntity"/> from the database.
    /// </summary>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether to track the changes of the retrieved entities.
    /// If set to <c>true</c>, entities are not tracked by the context.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of all entities of type <typeparamref name="TEntity"/>.
    /// </returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().ToListAsync();
        }

        return await Entities.ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves a collection of entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether the retrieved entities should be tracked by the context.
    /// If set to true, entities are retrieved without tracking.
    /// </param>
    /// <returns>An awaitable task result containing a collection of entities matching the predicate.</returns>
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
    /// Asynchronously adds a single entity to the database and commits the changes.
    /// </summary>
    /// <param name="entity">The entity to be added to the database.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether the entity should be removed from the tracking mechanism
    /// after being added. Defaults to false.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
    /// Asynchronously adds a collection of entities to the database context and persists the changes.
    /// </summary>
    /// <param name="entities">The collection of entities to be added.</param>
    /// <param name="asNoTracking">
    /// Specifies whether the entities should be detached from the change tracker after being added to the database.
    /// When true, the entities will not be tracked by the DbContext after addition.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
    /// Asynchronously removes an entity from the database and optionally removes it from tracking.
    /// </summary>
    /// <param name="entity">The entity to be removed from the database.</param>
    /// <param name="asNoTracking">Indicates whether the entity should be removed from tracking after being removed from the database.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
    /// Removes a collection of entities from the database asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to be removed.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether the entities should be removed from the tracking state
    /// after being deleted from the database.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
    /// Updates the specified entity asynchronously in the database and saves changes.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    /// <param name="asNoTracking">
    /// A boolean flag indicating whether the entity should be removed from the tracking state
    /// after being updated. If set to true, the entity is detached from the context.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task completes when the changes
    /// are saved to the database.
    /// </returns>
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
    /// Updates a collection of entities in the database asynchronously.
    /// </summary>
    /// <param name="entities">The collection of entities to be updated.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether to detach the entities from the change tracker
    /// after the update operation is complete. If true, the entities will not be tracked.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>
    /// Performs a bulk write operation by inserting a collection of entities into the specified table
    /// in batches to handle large datasets effectively.
    /// </summary>
    /// <param name="entities">The collection of entities to be written to the database.</param>
    /// <param name="tableName">The name of the database table where the entities will be written.</param>
    /// <param name="timeOutInSeconds">The timeout duration in seconds for the bulk write operation.</param>
    /// <returns>The total number of entities successfully written to the database.</returns>
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

    /// <summary>
    /// Removes the specified entity from the Entity Framework tracking mechanism,
    /// detaching it from the underlying DbContext.
    /// </summary>
    /// <param name="entity">The entity to be removed from the change tracker.</param>
    private void RemoveFromTracking(TEntity entity)
    {
        DataBase.Entry(entity).State = EntityState.Detached;
    }

    /// <summary>
    /// Removes the specified entity instances from the EF Core change tracker to prevent further tracking by the context.
    /// </summary>
    /// <param name="entities">A list of entities to be detached from the change tracker.</param>
    private void RemoveFromTracking(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            DataBase.Entry(entity).State = EntityState.Detached;
        }
    }
}