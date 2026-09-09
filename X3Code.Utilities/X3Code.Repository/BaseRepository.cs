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
/// <typeparam name="TContext">
/// The context this repository works on. Naming it makes the repository resolvable from a DI
/// container that has more than one context registered - a constructor asking for the base type
/// <see cref="DbContext"/> cannot be satisfied unless the application registers that separately,
/// and doing so is what quietly lets a repository end up in a different context than the unit of
/// work it is supposed to take part in.
/// </typeparam>
public class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    /// <summary>
    /// Entities per SaveChanges when no batch size is given.
    /// </summary>
    private const int DefaultBatchSize = 1000;

    /// <summary>
    /// Provides a base implementation for repository operations across entities of a specific type.
    /// </summary>
    public BaseRepository(TContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        DataBase = context;
        Entities = context.Set<TEntity>();
    }

    /// <summary>
    /// Represents the database context associated with the repository.
    /// </summary>
    /// <remarks>
    /// This property provides access to the underlying context used by the repository.
    /// It is intended for internal use within the repository to manage database operations.
    /// </remarks>
    protected TContext DataBase { get; }

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
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the entity
    /// that matches the predicate, or null if no such entity is found.
    /// </returns>
    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().SingleOrDefaultAsync(predicate, cancellationToken);
        }

        return await Entities.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves all entities of type <typeparamref name="TEntity"/> from the database.
    /// </summary>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether to track the changes of the retrieved entities.
    /// If set to <c>true</c>, entities are not tracked by the context.
    /// </param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of all entities of type <typeparamref name="TEntity"/>.
    /// </returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().ToListAsync(cancellationToken);
        }

        return await Entities.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously retrieves a collection of entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether the retrieved entities should be tracked by the context.
    /// If set to true, entities are retrieved without tracking.
    /// </param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>An awaitable task result containing a collection of entities matching the predicate.</returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }

        return await Entities.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously adds a single entity to the database and commits the changes.
    /// </summary>
    /// <param name="entity">The entity to be added to the database.</param>
    /// <param name="asNoTracking">
    /// A boolean value indicating whether the entity should be removed from the tracking mechanism
    /// after being added. Defaults to false.
    /// </param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(TEntity entity, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        await Entities.AddAsync(entity, cancellationToken);
        await DataBase.SaveChangesAsync(cancellationToken);

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
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var asList = entities.ToList();

        await Entities.AddRangeAsync(asList, cancellationToken);
        await DataBase.SaveChangesAsync(cancellationToken);

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
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RemoveAsync(TEntity entity, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        Entities.Remove(entity);
        await DataBase.SaveChangesAsync(cancellationToken);

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
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RemoveAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var asList = entities.ToList();

        Entities.RemoveRange(asList);
        await DataBase.SaveChangesAsync(cancellationToken);

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
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task completes when the changes
    /// are saved to the database.
    /// </returns>
    public async Task UpdateAsync(TEntity entity, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        Entities.Update(entity);
        await DataBase.SaveChangesAsync(cancellationToken);

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
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var asList = entities.ToList();

        Entities.UpdateRange(asList);
        await DataBase.SaveChangesAsync(cancellationToken);

        if (asNoTracking)
        {
            RemoveFromTracking(asList.ToList());
        }
    }

    /// <summary>
    /// Writes a collection of entities in batches, so a large set does not go into the database
    /// as one oversized command.
    /// </summary>
    /// <param name="entities">The collection of entities to be written to the database.</param>
    /// <param name="timeOutInSeconds">Command timeout for the write. 0 keeps the context's current setting.</param>
    /// <param name="batchSize">How many entities go into one SaveChanges. Defaults to 1000.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>The total number of entities successfully written to the database.</returns>
    /// <remarks>
    /// Each batch is saved on its own. If batch five fails, batches one to four are already
    /// committed - the name does not promise atomicity. Wrap the call in an
    /// <see cref="IUnitOfWork"/> scope when you need all or nothing.
    /// <para>
    /// There used to be a tableName parameter here. It was never read and could not be honoured:
    /// Entity Framework writes to the table the entity is mapped to.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="batchSize"/> is not positive.</exception>
    public async Task<int> BulkWrite(IEnumerable<TEntity> entities, int timeOutInSeconds = 0, int batchSize = DefaultBatchSize, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);

        var entityList = entities.ToList();
        if (entityList.Count == 0)
        {
            return 0;
        }

        var previousTimeout = DataBase.Database.GetCommandTimeout();
        if (timeOutInSeconds > 0)
        {
            DataBase.Database.SetCommandTimeout(timeOutInSeconds);
        }

        try
        {
            var totalWritten = 0;

            for (var i = 0; i < entityList.Count; i += batchSize)
            {
                var batch = entityList.Skip(i).Take(batchSize).ToList();

                await Entities.AddRangeAsync(batch, cancellationToken);
                totalWritten += batch.Count;

                await DataBase.SaveChangesAsync(cancellationToken);

                // Detach to avoid tracking issues
                RemoveFromTracking(batch);
            }

            return totalWritten;
        }
        finally
        {
            if (timeOutInSeconds > 0)
            {
                DataBase.Database.SetCommandTimeout(previousTimeout);
            }
        }
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

/// <summary>
/// The repository without naming its context. Same behavior as
/// <see cref="BaseRepository{TEntity, TContext}"/>, it just asks for the base
/// <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity to be managed by the repository. Must be a class.
/// </typeparam>
/// <remarks>
/// Kept for existing code. A container can only satisfy this when <see cref="DbContext"/> itself
/// is registered, which is ambiguous as soon as there is more than one context - name the context
/// via <see cref="BaseRepository{TEntity, TContext}"/> instead.
/// </remarks>
public class BaseRepository<TEntity> : BaseRepository<TEntity, DbContext> where TEntity : class
{
    /// <summary>
    /// Takes the context the repository works on.
    /// </summary>
    /// <param name="context">The context. Has to be the same instance the unit of work got.</param>
    public BaseRepository(DbContext context) : base(context)
    {
    }
}
