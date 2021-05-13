using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace X3Code.Infrastructure.RavenDb
{
    public interface IRavenUnitOfWork
    {
        void Complete();
        T SingleOrDefault<T>(Func <T, bool> func);
        void Store<T>(T entity);
        void Delete<T>(T entity);
        IEnumerable<T> Where<T>(Func <T, bool> func);
        Task<T> SingleOrDefaultAsync<T>(Expression<Func <T, bool>> func);
        Task StoreAsync<T>(T entity);
        Task DeleteAsync<T>(T entity);
    }
}