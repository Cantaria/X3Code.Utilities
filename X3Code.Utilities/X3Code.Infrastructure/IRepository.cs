using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace X3Code.Infrastructure
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAllAsNoTracking();

        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetAllAsNoTracking(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);

        void AddAll(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        void RemoveAll(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void UpdateAll(IEnumerable<TEntity> entities);

		Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

		Task<IQueryable<TEntity>> GetAllAsync();

		Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

		Task AddAsync(TEntity entity);

		Task AddAllAsync(IEnumerable<TEntity> entities);

		Task RemoveAsync(TEntity entity);

		Task RemoveAllAsync(IEnumerable<TEntity> entities);

		Task UpdateAsync(TEntity entity);
		
		Task UpdateAllAsync(IEnumerable<TEntity> entities);
	}
}
