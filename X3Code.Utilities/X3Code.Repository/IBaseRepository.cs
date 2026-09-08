using System.Linq.Expressions;

namespace X3Code.Repository;

public interface IBaseRepository<TEntity>
{
	TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
	IEnumerable<TEntity> GetAll(bool asNoTracking = false);
	IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
	IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
	IQueryable<TEntity> Query(bool asNoTracking = false);
	void Add(TEntity entity, bool asNoTracking = false);
	void AddAll(IEnumerable<TEntity> entities, bool asNoTracking = false);
	void Remove(TEntity entity, bool asNoTracking = false);
	void RemoveAll(IEnumerable<TEntity> entities, bool asNoTracking = false);
	void Update(TEntity entity, bool asNoTracking = false);
	void UpdateAll(IEnumerable<TEntity> entities, bool asNoTracking = false);
	Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default);
	Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false, CancellationToken cancellationToken = default);
	Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default);
	Task AddAsync(TEntity entity, bool asNoTracking = false, CancellationToken cancellationToken = default);
	Task AddAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false, CancellationToken cancellationToken = default);
	Task RemoveAsync(TEntity entity, bool asNoTracking = false, CancellationToken cancellationToken = default);
	Task RemoveAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false, CancellationToken cancellationToken = default);
	Task UpdateAsync(TEntity entity, bool asNoTracking = false, CancellationToken cancellationToken = default);
	Task UpdateAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false, CancellationToken cancellationToken = default);
	Task<int> BulkWrite(IEnumerable<TEntity> entities, string tableName, int timeOutInSeconds, CancellationToken cancellationToken = default);
}