using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;

namespace X3Code.Infrastructure.RavenDb
{
    public class RavenUnitOfWork : IDisposable, IRavenUnitOfWork
    {
        private RavenDbContext _context;
        private readonly IEnumerable<string> _connectionString;
        private readonly string _databaseName;
        private readonly object _syncContext = new object();

        #region Construction

        public RavenUnitOfWork(IEnumerable<string> nodeUrls, string databaseName)
        {
            _connectionString = nodeUrls;
            _databaseName = databaseName;
        }

        private RavenDbContext Context
        {
            get
            {
                lock (_syncContext)
                {
                    if (_context == null)
                    {
                        _context = new RavenDbContext(_connectionString, _databaseName);
                    }
                    return _context;
                }
            }
        }

        #endregion

        public T SingleOrDefault<T>(Func <T, bool> func)
        {
            using var session = Context.Get.OpenSession();
            return session.Query<T>().SingleOrDefault(func);
        }

        public IEnumerable<T> Where<T>(Func <T, bool> func)
        {
            using var session = Context.Get.OpenSession();
            return session.Query<T>().Where(func).ToList();
        }

        public void Store<T>(T entity)
        {
            using var session = Context.Get.OpenSession();
            session.Store(entity);
            session.SaveChanges();
        }

        public void Delete<T>(T entity)
        {
            using var session = Context.Get.OpenSession();
            session.Delete(entity);
            session.SaveChanges();
        }
        
        public async Task<T> SingleOrDefaultAsync<T>(Expression<Func <T, bool>> func)
        {
            using var session = Context.Get.OpenAsyncSession();
            return await session.Query<T>().SingleOrDefaultAsync(func, CancellationToken.None);
        }

        public async Task StoreAsync<T>(T entity)
        {
            using var session = Context.Get.OpenAsyncSession();
            await session.StoreAsync(entity);
            await session.SaveChangesAsync();
        }
        
        public async Task DeleteAsync<T>(T entity)
        {
            using var session = Context.Get.OpenAsyncSession();
            session.Delete(entity);
            await session.SaveChangesAsync();
        }

        public void Complete()
        {
            Dispose();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}