using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;

namespace X3Code.Infrastructure.RavenDb
{
    /// <summary>
    /// Creates the DocumentStore for accessing the RavenDB
    /// </summary>
    public class RavenDbContext : IDisposable
    {
        private readonly IEnumerable<string> _clusterNodeUrls;
        private readonly string _databaseName;
        private readonly DocumentConventions _conventions;
        private IDocumentStore _store;

        /// <summary>
        /// Creates the DocumentStore for accessing the RavenDB
        /// </summary>
        /// <param name="clusterNodeUrls">The URLs to the cluster node</param>
        /// <param name="databaseName">The name of the database</param>
        /// <param name="conventions">DocumentConventions</param>
        public RavenDbContext(IEnumerable<string> clusterNodeUrls, string databaseName, DocumentConventions conventions)
        {
            _clusterNodeUrls = clusterNodeUrls;
            _databaseName = databaseName;
            _conventions = conventions;
        }
        
        /// <summary>
        /// Creates the DocumentStore for accessing the RavenDB, sets some default DocumentConventions
        /// </summary>
        /// <param name="clusterNodeUrls">The URLs to the cluster node</param>
        /// <param name="databaseName">The name of the database</param>
        public RavenDbContext(IEnumerable<string> clusterNodeUrls, string databaseName)
        {
            _clusterNodeUrls = clusterNodeUrls;
            _databaseName = databaseName;
            _conventions = new DocumentConventions
            {
                MaxNumberOfRequestsPerSession = 10,
                UseOptimisticConcurrency = true
            };
        }

        /// <summary>
        /// Gets the DocumentStore for the RavenDB
        /// </summary>
        public IDocumentStore Get
        {
            get
            {
                if (_store == null) _store = CreateDocumentStore();
                return _store;
            }
        }
    
        private IDocumentStore CreateDocumentStore()
        {
            return new DocumentStore()
            {
                Urls = _clusterNodeUrls.ToArray(),
                Database = _databaseName,
                Conventions = _conventions
            }.Initialize();
        }

        public void Dispose()
        {
            _store?.Dispose();
        }
    }
}