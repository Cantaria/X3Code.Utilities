using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace X3Code.Infrastructure.RavenDb
{
    public class GenericRepository<T> where T : Entity
    {
        protected IRavenUnitOfWork UnitOfWork;

        protected GenericRepository(IRavenUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
        
        public T SingleOrDefault(Func<T, bool> func)
        {
            return UnitOfWork.SingleOrDefault(func);
        }

        public IEnumerable<T> Where(Func <T, bool> func)
        {
            return UnitOfWork.Where(func);
        }

        public void Store(T entity)
        {
            UnitOfWork.Store(entity);
        }

        public void Delete(T entity)
        {
            UnitOfWork.Delete(entity);
        }
        
        public async Task<T> SingleOrDefaultAsync(Expression<Func <T, bool>> func)
        {
            return await UnitOfWork.SingleOrDefaultAsync(func);
        }

        public async Task StoreAsync(T entity)
        {
            await UnitOfWork.StoreAsync(entity);
        }
    }
}