using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace X3Code.Infrastructure.RavenDb
{
    public interface IRavenUnitOfWork
    {
        T SingleOrDefault<T>(Func <T, bool> func);
        IEnumerable<T> Where<T>(Func <T, bool> func);
        void Store<T>(T entity);
        Task<T> SingleOrDefaultAsync<T>(Expression<Func <T, bool>> func);
        Task StoreAsync<T>(T entity);
        void Complete();
        void Delete<T>(T entity);
    }
}