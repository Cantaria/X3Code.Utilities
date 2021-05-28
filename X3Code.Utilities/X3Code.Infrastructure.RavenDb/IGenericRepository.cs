using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace X3Code.Infrastructure.RavenDb
{
    public interface IGenericRepository<T> where T : class, IEntity, new()
    {
        T Get(Func<T, bool> func);
        IEnumerable<T> GetAll(Func <T, bool> func);
        void Store(T entity);
        void Delete(T entity);
        Task<T> GetAsync(Expression<Func <T, bool>> func);
        Task StoreAsync(T entity);
    }
}