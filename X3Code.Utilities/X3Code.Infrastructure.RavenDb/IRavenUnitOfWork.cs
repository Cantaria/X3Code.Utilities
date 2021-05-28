using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;

namespace X3Code.Infrastructure.RavenDb
{
    public interface IRavenUnitOfWork
    {
        void Complete();
        T Get<T>(Func <T, bool> func) where T : class, new();
        void Store<T>(T entity) where T : class, new();
        void Delete<T>(T entity) where T : class, new();
        IEnumerable<T> GetAll<T>(Func <T, bool> func) where T : class, new();
        Task<T> GetAsync<T>(Expression<Func <T, bool>> func) where T : class, new();
        Task StoreAsync<T>(T entity) where T : class, new();
        Task DeleteAsync<T>(T entity) where T : class, new();
        IDocumentSession GetSession();
        IAsyncDocumentSession GetAsyncSession();
    }
}