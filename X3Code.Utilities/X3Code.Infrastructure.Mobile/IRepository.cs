using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace X3Code.Infrastructure.Mobile;

/// <summary>
/// Represents a repository for accessing and manipulating entities of type TEntity.
/// </summary>
/// <typeparam name="TEntity">The type of entities stored in the repository.</typeparam>
public interface IRepository<TEntity> where TEntity : IEntity
{
	/// <summary>
	/// Retrieves a single entity from the database based on the specified predicate.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="predicate">The predicate used to filter the entities.</param>
	/// <param name="asNoTracking">Determines whether to track changes for the retrieved entity. The default value is false.</param>
	/// <returns>The retrieved entity that matches the given predicate.</returns>
	TEntity? Get(Expression<Func<TEntity?, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Retrieves all entities from the database.
	/// </summary>
	/// <param name="asNoTracking">Indicates whether the retrieved entities should be tracked by the database context or not.</param>
	/// <returns>An IEnumerable of type TEntity containing all the retrieved entities.</returns>
	IEnumerable<TEntity> GetAll(bool asNoTracking = false);

	/// <summary>
	/// Retrieves all entities from the data source that satisfy the specified predicate.
	/// </summary>
	/// <param name="predicate">The predicate that specifies the condition to match.</param>
	/// <param name="asNoTracking">Determines whether to track changes for the returned entities. Default is false.</param>
	/// <returns>
	/// An IEnumerable<TEntity> that contains the entities satisfying the predicate.
	/// </returns>
	IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Executes a query on the specified entity using the given predicate.
	/// </summary>
	/// <typeparam name="TEntity">The entity type.</typeparam>
	/// <param name="predicate">The predicate used to filter the entities.</param>
	/// <param name="asNoTracking">Indicates whether the entities should be retrieved as non-tracking.</param>
	/// <returns>An <see cref="IQueryable{TEntity}"/> representing the result of the query.</returns>
	IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Performs a query on the database.
	/// </summary>
	/// <typeparam name="TEntity">The type of entity to query.</typeparam>
	/// <param name="asNoTracking">Specifies whether to track the query results or not. Default is false.</param>
	/// <returns>The query results as an IQueryable collection of TEntity.</returns>
	/// <remarks>
	/// Use this method to perform a query on the database. The query results will be returned as an IQueryable collection of TEntity.
	/// By default, the query results will be tracked by the context for change detection and can be used for updating the database.
	/// If you want to just retrieve the data without tracking changes, you can set the asNoTracking parameter to true.
	/// </remarks>
	IQueryable<TEntity> Query(bool asNoTracking = false);

	/// <summary>
	/// Adds a new entity to the database.
	/// </summary>
	/// <param name="entity">The entity to be added.</param>
	/// <param name="asNoTracking">Optional. Specifies whether the added entity should be tracked by the DbContext or not. Default value is false.</param>
	/// <remarks>
	/// This method adds the specified entity to the database. If the asNoTracking parameter is set to true, the entity will not be tracked by the DbContext.
	/// Otherwise, the entity will be tracked and any changes made to it will be saved to the database when SaveChanges() is called on the DbContext.
	/// </remarks>
	/// <example>
	/// Here is an example of how to use the Add() method:
	/// <code>
	/// var newUser = new User { Name = "John Doe", Age = 30 };
	/// repository.Add(newUser);
	/// repository.SaveChanges();
	/// </code>
	/// </example>
	void Add(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Adds the specified collection of entities to the current context.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entities.</typeparam>
	/// <param name="entities">The collection of entities to be added.</param>
	/// <param name="asNoTracking">Optional. Determines whether the added entities should be tracked by the context.</param>
	/// <remarks>
	/// This method adds the specified collection of entities to the current context for future operations.
	/// By default, the added entities are tracked by the context, which means any changes made to the entities will be detected by the context.
	/// To optimize performance, you can set the <paramref name="asNoTracking"/> parameter to true, which will prevent the context from tracking the added entities.
	/// </remarks>
	void AddAll(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Removes the specified entity from the database.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="entity">The entity to remove.</param>
	/// <param name="asNoTracking">Determines whether the entity should be tracked or not. Default is false.</param>
	/// <remarks>
	/// <para>
	/// The Remove method allows you to remove a specific entity from the database.
	/// </para>
	/// <para>
	/// By default, the entity will be tracked by the database context after removal.
	/// To disable tracking, set the asNoTracking parameter to true.
	/// </para>
	/// </remarks>
	void Remove(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Removes all entities from the database that match the specified criteria.
	/// </summary>
	/// <typeparam name="TEntity">The type of entities to be removed.</typeparam>
	/// <param name="entities">The collection of entities to be removed.</param>
	/// <param name="asNoTracking">Specifies whether the entities should be tracked by the context after removal.
	/// By default, entities will be tracked.</param>
	/// <remarks>
	/// This method removes all entities from the database that match the specified criteria.
	/// The <paramref name="entities"/> collection should contain the entities that need to be removed.
	/// By default, the entities will be tracked by the context after removal, which means any changes to the
	/// entities after removal will be detected and saved to the database. If the <paramref name="asNoTracking"/>
	/// parameter is set to true, the removed entities will not be tracked, and any changes made to them after
	/// removal will not be saved to the database.
	/// </remarks>
	void RemoveAll(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Updates an entity in the database.
	/// </summary>
	/// <typeparam name="TEntity">The type of entity to be updated.</typeparam>
	/// <param name="entity">The entity to be updated.</param>
	/// <param name="asNoTracking">Indicates whether to track changes made to the entity or not. Default value is false.</param>
	/// <remarks>
	/// This method updates the specified entity in the database.
	/// If <paramref name="asNoTracking"/> is set to true, changes made to the entity will not be tracked and saved to the database.
	/// </remarks>
	/// <example>
	/// This code shows an example of updating an entity:
	/// <code>
	/// // Create a new instance of the entity to be updated
	/// var entityToUpdate = new MyEntity { Id = 1, Name = "UpdatedEntity" };
	/// // Update the entity in the database
	/// Update(entityToUpdate, true);
	/// </code>
	/// </example>
	void Update(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Updates all the entities in the provided enumerable collection.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entities.</typeparam>
	/// <param name="entities">The enumerable collection of entities to update.</param>
	/// <param name="asNoTracking">Optional. Specifies whether to disable change tracking for the entities being updated. Default is false.</param>
	/// <remarks>
	/// This method updates all the entities in the provided collection.
	/// If the 'asNoTracking' parameter is set to true, change tracking will be disabled,
	/// which means that the entities will be treated as read-only and any changes made to them
	/// will not be persisted to the data source.
	/// </remarks>
	void UpdateAll(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Retrieves a single entity asynchronously based on the specified predicate expression.
	/// </summary>
	/// <param name="predicate">The predicate expression used to filter the entities.</param>
	/// <param name="asNoTracking">Whether to disable change tracking for the retrieved entity. Default is false.</param>
	/// <returns>A task representing the asynchronous operation. The task result represents the retrieved entity.</returns>
	/// <example>
	/// This example retrieves a user entity with the specified user ID.
	/// <code>
	/// Expression<Func<User, bool>> predicate = u => u.UserId == 100;
	/// User user = await userRepository.GetAsync(predicate);
	/// </code>
	/// </example>
	/// <remarks>
	/// The retrieved entity will be either tracked or not tracked based on the value of the asNoTracking parameter.
	/// By default, change tracking is enabled.
	/// </remarks>
	Task<TEntity?> GetAsync(Expression<Func<TEntity?, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Retrieves all entities asynchronously from the database.
	/// </summary>
	/// <typeparam name="TEntity">The entity type.</typeparam>
	/// <param name="asNoTracking">
	/// If set to <c>true</c>, the entities will not be tracked by the DbContext.
	/// </param>
	/// <returns>
	/// A task representing the asynchronous operation. The task result contains
	/// an enumerable collection of entities of type <typeparamref name="TEntity"/>.
	/// </returns>
	Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false);

	/// <summary>
	/// Retrieves all entities asynchronously based on the provided predicate.
	/// </summary>
	/// <param name="predicate">The expression predicate used to filter the entities.</param>
	/// <param name="asNoTracking">A flag indicating whether to include tracking information for the entities. Default is false.</param>
	/// <returns>A task that represents the asynchronous operation and returns an enumerable collection of entities.</returns>
	/// <remarks>
	/// This method retrieves all entities from the data source that satisfy the specified predicate.
	/// By default, tracking information is included for the entities. However, by setting the <paramref name="asNoTracking"/> parameter to true, tracking can be disabled.
	/// </remarks>
	Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

	/// <summary>
	/// Adds a new entity to the database asynchronously.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="entity">The entity to be added to the database.</param>
	/// <param name="asNoTracking">Indicates whether to track the entity after adding it. Default is false.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the entity that was added to the database.</returns>
	Task AddAsync(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Adds multiple entities to the database asynchronously.
	/// </summary>
	/// <typeparam name="TEntity">The type of entity.</typeparam>
	/// <param name="entities">The collection of entities to add.</param>
	/// <param name="asNoTracking">Indicates whether the entities should be added without tracking.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	/// <remarks>
	/// This method adds the specified collection of entities to the database.
	/// By default, the entities are added with tracking. If <paramref name="asNoTracking"/> is set to true,
	/// the entities are added without tracking, which can improve performance in certain scenarios.
	/// </remarks>
	Task AddAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Removes the specified entity from the database asynchronously.
	/// </summary>
	/// <param name="entity">The entity to be removed.</param>
	/// <param name="asNoTracking">Indicates whether to prevent tracking of the entity after removal. The default value is false.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	/// <remarks>
	/// This method removes the specified entity from the database. If <paramref name="asNoTracking"/> is set to true, the entity will not be tracked by the database context after removal
	/// .
	/// </remarks>
	Task RemoveAsync(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Removes all entities asynchronously from the database that match the given collection of entities.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entities.</typeparam>
	/// <param name="entities">A collection of entities to be removed.</param>
	/// <param name="asNoTracking">A boolean indicating whether to disable change tracking for the removed entities. Default is false.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task RemoveAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false);

	/// <summary>
	/// Updates the specified entity asynchronously.
	/// </summary>
	/// <param name="entity">The entity to update.</param>
	/// <param name="asNoTracking">Indicates whether to track the entity after updating. Default is false.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task UpdateAsync(TEntity entity, bool asNoTracking = false);

	/// <summary>
	/// Updates multiple entities asynchronously in the database.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	/// <param name="entities">The collection of entities to be updated.</param>
	/// <param name="asNoTracking">A boolean value indicating whether tracking should be enabled or disabled for the entities being updated. Default is false.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	Task UpdateAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false);
}