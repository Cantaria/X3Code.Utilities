using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;

namespace X3Code.Infrastructure.RavenDb
{
    public interface IRavenUnitOfWork
    {
        void Complete();
        T Get<T>(Func <T, bool> func);
        void Store<T>(T entity);
        void Delete<T>(T entity);
        IEnumerable<T> GetAll<T>(Func <T, bool> func);
        Task<T> GetAsync<T>(Expression<Func <T, bool>> func);
        Task StoreAsync<T>(T entity);
        Task DeleteAsync<T>(T entity);
        IDocumentSession GetSession();
        IAsyncDocumentSession GetAsyncSession();
    }
}