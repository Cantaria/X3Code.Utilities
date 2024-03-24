using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace X3Code.Infrastructure;

/// <summary>
/// Represents a repository for accessing and managing entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The type of entity being managed by the repository.</typeparam>
public interface IRepository<TEntity> where TEntity : IEntity
{
	/// <summary>
	/// Retrieves a single entity based on the specified predicate.
	/// </summary>
	/// <typeparam name="TEntity">The type of entity to retrieve.</typeparam>
	/// <param name="predicate">The expression used to filter the entities.</param>
	/// <param name="asNoTracking">Indicates whether to track the retrieved entity or not. Default is false.</param>
	/// <returns>The retrieved entity if found, otherwise null.</returns>
	TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Retrieves all entities of type <typeparamref name="TEntity"/>.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="asNoTracking">Determines whether to enable "no tracking" for the retrieved entities.</param>
	/// <returns>An IEnumerable of <typeparamref name="TEntity"/> containing all the entities.</returns>
	IEnumerable<TEntity> GetAll(bool asNoTracking = false);

	/// <summary>
	/// Retrieves all entities from the database that match the specified predicate.
	/// </summary>
	/// <param name="predicate">The predicate used to filter the entities.</param>
	/// <param name="asNoTracking">Flag indicating whether the entities should be tracked or not.</param>
	/// <returns>
	/// An <see cref="IEnumerable{TEntity}"/> containing all entities that match the predicate.
	/// </returns>
	IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Queries the data from the database using the specified predicate and optional asNoTracking flag.
	/// </summary>
	/// <param name="predicate">The predicate to filter the data.</param>
	/// <param name="asNoTracking">Determines whether to track the entities or not.</param>
	IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Retrieves an <see cref="IQueryable"/> object for querying entities of type <typeparamref name="TEntity"/>.
	/// </summary>
	/// <param name="asNoTracking">Specifies whether the query should be performed with or without change tracking.</param>
	/// <returns>An <see cref="IQueryable"/> object representing the query.</returns>
	IQueryable<TEntity> Query(bool asNoTracking = false);

	/// <summary>
	/// Adds a new entity to the database.
	/// </summary>
	/// <typeparam name="TEntity">The type of entity to add.</typeparam>
	/// <param name="entity">The entity to be added.</param>
	/// <param name="asNoTracking">Indicates whether to add the entity as no-tracking.</param>
	/// <remarks>
	/// The <paramref name="entity"/> will be added to the database. If <paramref name="asNoTracking"/> is set to true, the entity will not be tracked by the context after being added.
	/// </remarks>
	void Add(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Adds a collection of entities to the context.
	/// </summary>
	/// <typeparam name="TEntity">The type of entities in the collection.</typeparam>
	/// <param name="entities">The collection of entities to add.</param>
	/// <param name="asNoTracking">Flag indicating whether to track the added entities or not. By default, entities are tracked.</param>
	void AddAll(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Removes the specified entity from the database.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="entity">The entity to be removed.</param>
	/// <param name="asNoTracking">Indicates whether to retrieve the entity as no-tracking.</param>
	/// <remarks>
	/// This method removes the specified entity from the database. If the asNoTracking parameter is set to true,
	/// the entity will be retrieved as no-tracking before removing it. Otherwise, the entity will be retrieved as
	/// tracking and removed from the database context.
	/// </remarks>
	void Remove(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Removes all entities from the Entity Framework context.
	/// </summary>
	/// <param name="entities">The collection of entities to be removed.</param>
	/// <param name="asNoTracking">Determines whether the removed entities should be tracked by the context.</param>
	/// <remarks>
	/// This method removes all the specified entities from the Entity Framework context.
	/// By default, the removed entities are tracked by the context.
	/// Set asNoTracking to true if you do not want the entities to be tracked after removal.
	/// </remarks>
	/// <seealso cref="IEnumerable{TEntity}"/>
	/// <exception cref="ArgumentNullException">Thrown when the entities parameter is null.</exception>
	void RemoveAll(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Updates the specified entity in the database.
	/// </summary>
	/// <param name="entity">The entity to update.</param>
	/// <param name="asNoTracking">Indicates whether to use the DbContext's tracking feature. If set to true, changes to the entity will not be tracked by the context.</param>
	/// <remarks>
	/// This method updates the entity in the database. If <paramref name="asNoTracking"/> is set to true, the entity will not be tracked for changes by the DbContext, allowing for better
	/// performance in certain scenarios.
	/// </remarks>
	void Update(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Updates the specified entities in the database.
	/// </summary>
	/// <param name="entities">The collection of entities to be updated.</param>
	/// <param name="asNoTracking">Optional. Specifies whether the entities should be updated in a "no tracking" query mode.</param>
	/// <remarks>
	/// This method updates the specified entities in the database. If the <paramref name="asNoTracking"/> parameter is set to true,
	/// the entities will be queried and updated in a "no tracking" query mode, meaning that changes made to the entities after retrieval
	/// will not be reflected in the database upon update.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when the <paramref name="entities"/> parameter is null.</exception>
	/// <example>
	/// This example demonstrates how to use the UpdateAll method to update a collection of entities in the database:
	/// <code>
	/// var entities = new List&lt;TEntity&gt; { entity1, entity2, entity3 };
	/// dbContext.UpdateAll(entities);
	/// </code>
	/// </example>
	void UpdateAll(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Asynchronously retrieves entities from the database that match the specified predicate.
	/// </summary>
	/// <param name="predicate">The predicate used to filter the entities.</param>
	/// <param name="asNoTracking">Indicates whether to disable or enable change tracking for the queried entities. Default is false.</param>
	/// <returns>A task representing the asynchronous operation. The task result contains the retrieved entity.</returns>
	/// <remarks>This method is used to retrieve entities from the database based on the specified predicate. By default,
	/// change tracking is enabled for the queried entities. Setting the asNoTracking parameter to true will
	/// disable the change tracking for the retrieved entities.</remarks>
	Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Retrieves all entities asynchronously from the database. </summary> <param name="asNoTracking">Specifies whether to track the entities or not. Default is false.</param> <returns>A task that represents the asynchronous operation and contains the collection of entities retrieved from the database.</returns>
	/// /
	Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false);

	/// <summary>
	/// Retrieves all entities of type <typeparamref name="TEntity"/> that satisfy the given predicate asynchronously.
	/// </summary>
	/// <param name="predicate">The predicate used to filter the entities.</param>
	/// <param name="asNoTracking">Determines whether to track the entities or not. By default, tracking is enabled (false).</param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains an enumerable collection of entities
	/// that satisfy the given predicate. If no entities are found, an empty collection is returned.
	/// </returns>
	Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Adds the specified entity to the context asynchronously.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="entity">The entity to be added.</param>
	/// <param name="asNoTracking">Flag indicating whether to track the entity or not (optional, default is false).</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	/// <remarks>
	/// This method adds the specified entity to the context and saves it to the underlying data store.
	/// The <paramref name="asNoTracking"/> flag can be used to determine whether or not the entity
	/// should be tracked by the context. By default, tracking is enabled.
	/// </remarks>
	Task AddAsync(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Adds a collection of entities asynchronously to the database.
	/// </summary>
	/// <typeparam name="TEntity">The type of entities to add.</typeparam>
	/// <param name="entities">The collection of entities to add.</param>
	/// <param name="asNoTracking">Optional. Indicates whether the entities should be added as no tracking. Default is false.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	/// <remarks>
	/// This method can be used to add a collection of entities to the database. The entities will be persisted asynchronously.
	/// By default, the entities are added with tracking enabled, but you can pass <paramref name="asNoTracking"/> as true to disable tracking for the added entities.
	/// </remarks>
	Task AddAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Remove an entity from the database asynchronously.
	/// </summary>
	/// <typeparam name="TEntity">The type of entity to remove.</typeparam>
	/// <param name="entity">The entity to remove.</param>
	/// <param name="asNoTracking">Optional. Specify whether to track the entity after removal. Default is false.</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	/// <remarks>
	/// The <paramref name="entity"/> will be removed from the database. If <paramref name="asNoTracking"/> is true,
	/// the entity will not be tracked by the data context after removal. Otherwise, it will be tracked and any changes
	/// made to the entity will be reflected in the database.
	/// </remarks>
	Task RemoveAsync(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Asynchronously removes multiple entities from the database.
	/// </summary>
	/// <typeparam name="TEntity">The type of entities to remove.</typeparam>
	/// <param name="entities">The collection of entities to remove.</param>
	/// <param name="asNoTracking">A boolean value indicating whether to disable change tracking for the removed entities. Default is false.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	/// <remarks>
	/// This method removes the specified entities from the database.
	/// It operates in an asynchronous manner, allowing for better performance and responsiveness.
	/// By default, change tracking is enabled for the removed entities.
	/// You can set the <paramref name="asNoTracking"/> parameter to true to disable change tracking.
	/// </remarks>
	Task RemoveAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Updates the specified entity asynchronously.
	/// </summary>
	/// <typeparam name="TEntity">The type of entity to update.</typeparam>
	/// <param name="entity">The entity to update.</param>
	/// <param name="asNoTracking">Indicates whether to track changes or not.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	/// <remarks>
	/// The UpdateAsync method updates the specified entity asynchronously.
	/// It takes an entity object of type TEntity and updates it in the underlying data store.
	/// By default, changes made to the entity will be tracked by the context.
	/// The asNoTracking parameter can be set to true to disable change tracking.
	/// </remarks>
	Task UpdateAsync(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Updates multiple entities asynchronously.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entities.</typeparam>
	/// <param name="entities">The collection of entities to update.</param>
	/// <param name="asNoTracking">Optional. Specifies whether to track the entities in the context or not.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task UpdateAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Performs a bulk write operation by inserting or updating multiple entities in the specified table.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entities.</typeparam>
	/// <param name="entities">The collection of entities to be inserted or updated.</param>
	/// <param name="tableName">The name of the table where the entities will be inserted or updated.</param>
	/// <param name="timeOutInSeconds">The timeout value in seconds for the bulk write operation.</param>
	/// <returns>A task that represents the asynchronous bulk write operation. The task result represents the number of entities that were successfully inserted or updated.</returns>
	Task<int> BulkWrite(IEnumerable<TEntity> entities, string tableName, int timeOutInSeconds);
}