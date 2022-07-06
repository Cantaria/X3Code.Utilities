using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace X3Code.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        TEntity Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

        IEnumerable<TEntity> GetAll(bool asNoTracking = false);

        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

        void Add(TEntity entity);

        void AddAll(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        void RemoveAll(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void UpdateAll(IEnumerable<TEntity> entities);

		Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

		Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false);

		Task AddAsync(TEntity entity);

		Task AddAllAsync(IEnumerable<TEntity> entities);

		Task RemoveAsync(TEntity entity);

		Task RemoveAllAsync(IEnumerable<TEntity> entities);

		Task UpdateAsync(TEntity entity);
		
		Task UpdateAllAsync(IEnumerable<TEntity> entities);
	}
}
