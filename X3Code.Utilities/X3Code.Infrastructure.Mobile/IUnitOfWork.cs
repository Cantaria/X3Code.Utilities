using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace X3Code.Infrastructure.Mobile
{
	public interface IUnitOfWork
	{
		IQueryable<TEntity> Query<TEntity>() where TEntity : class, new();

		void Remove<TEntity>(TEntity entity) where TEntity : class, new();

		void Add<TEntity>(TEntity entity) where TEntity : class, IEntity, new();

		void AddRange<TEntity>(IEnumerable<TEntity> listOfEntities) where TEntity : class, new();

		void RemoveRange<TEntity>(IEnumerable<TEntity> listOfEntities) where TEntity : class, new();

		void RemoveRange<T>(Func<T, bool> predicate)  where T : class, new();

		Task RemoveRangeAsync<T>(Func<T, bool> predicate) where T : class, new();

		Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, new();

		Task AddAsync<TEntity>(TEntity entity) where TEntity : class, IEntity, new();

		Task AddRangeAsync<TEntity>(IEnumerable<TEntity> listOfEntities) where TEntity : class, new();

		Task RemoveRangeAsync<TEntity>(IEnumerable<TEntity> listOfEntities) where TEntity : class, new();

		void Complete();

	}
}