using System;
using System.Collections.Generic;
using System.Linq;

namespace X3Code.Infrastructure.RavenDb
{
    public class UnitOfWork : IDisposable
    {
        private RavenDbContext _context;
        private readonly IEnumerable<string> _connectionString;
        private readonly string _databaseName;
        private readonly object _syncContext = new object();

        #region Construction

        public UnitOfWork(IEnumerable<string> nodeUrls, string databaseName)
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

        public T DoSomething<T>(Func<T,T,T> func)
        {
            var session = Context.Get.OpenSession();
            return session.Query<T>().Aggregate(func);
        }
        
        public void Dispose()
        {
            
            _context.Dispose();
        }
    }
}