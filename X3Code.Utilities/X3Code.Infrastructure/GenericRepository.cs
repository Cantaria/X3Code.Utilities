using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace X3Code.Infrastructure
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class, IEntity, new()
    {
		protected GenericRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected IUnitOfWork UnitOfWork { get; }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                return UnitOfWork.Query<T>().SingleOrDefault(predicate);
            }
        }

		public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
		{
			using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                return await UnitOfWork.Query<T>().SingleOrDefaultAsync(predicate);
            }
		}

		public IEnumerable<T> GetAll()
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                return UnitOfWork.Query<T>().ToList();
            }
        }

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			using (new UnitOfWorkLifeCycle(UnitOfWork))
			{
				return await UnitOfWork.Query<T>().ToListAsync();
			}
		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                return UnitOfWork.Query<T>().Where(predicate).ToList();
            }
        }

		public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
		{
			using (new UnitOfWorkLifeCycle(UnitOfWork))
			{
				return await UnitOfWork.Query<T>().Where(predicate).ToListAsync();
			}
		}

		public void Add(T entity)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                UnitOfWork.Add(entity);
            }
        }

		public async Task AddAsync(T entity)
		{
			using (new UnitOfWorkLifeCycle(UnitOfWork))
			{
				await UnitOfWork.AddAsync(entity);
			}
		}

		public void AddAll(IEnumerable<T> entities)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                UnitOfWork.AddRange(entities);
            }
        }

		public async Task AddAllAsync(IEnumerable<T> entities)
		{
			using (new UnitOfWorkLifeCycle(UnitOfWork))
			{
				await UnitOfWork.AddRangeAsync(entities);
			}
		}

		public void Remove(T entity)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                UnitOfWork.Remove(entity);
            }
        }

		public async Task RemoveAsync(T entity)
		{
			using (new UnitOfWorkLifeCycle(UnitOfWork))
			{
				await UnitOfWork.DeleteAsync(entity);
			}
		}

		public void RemoveAll(IEnumerable<T> entities)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                UnitOfWork.RemoveRange(entities);
            }
        }

		public async Task RemoveAllAsync(IEnumerable<T> entities)
		{
			using (new UnitOfWorkLifeCycle(UnitOfWork))
			{
				await UnitOfWork.RemoveRangeAsync(entities);
			}
		}

		public void Update(T entity)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                UnitOfWork.Add(entity);
            }
        }

		public async Task UpdateAsync(T entity)
		{
			using (new UnitOfWorkLifeCycle(UnitOfWork))
			{
				await UnitOfWork.AddAsync(entity);
			}
		}
	}
}
