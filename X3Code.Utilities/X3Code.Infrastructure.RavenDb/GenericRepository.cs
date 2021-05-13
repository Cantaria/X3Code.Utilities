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
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                return UnitOfWork.SingleOrDefault(func);   
            }
        }

        public IEnumerable<T> Where(Func <T, bool> func)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                return UnitOfWork.Where(func);      
            }
        }

        public void Store(T entity)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                UnitOfWork.Store(entity);       
            }
        }

        public void Delete(T entity)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                UnitOfWork.Delete(entity);       
            }
        }
        
        public async Task<T> SingleOrDefaultAsync(Expression<Func <T, bool>> func)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                return await UnitOfWork.SingleOrDefaultAsync(func);      
            }
        }

        public async Task StoreAsync(T entity)
        {
            using (new UnitOfWorkLifeCycle(UnitOfWork))
            {
                await UnitOfWork.StoreAsync(entity);       
            }
        }
    }
}