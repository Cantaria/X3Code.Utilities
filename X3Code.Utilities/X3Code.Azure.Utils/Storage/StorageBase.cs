using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace X3Code.Azure.Utils.Storage;

/// <summary>
/// Base class for all Azure Storage types
/// </summary>
public abstract class StorageBase : IAzureStorage
{
    protected string ConnectionString { get; }
    protected string ContainerName { get; }

    /// <summary>
    /// The logger of this storage. Never null - it is the no-op logger when none was handed in.
    /// </summary>
    protected ILogger Logger { get; }

    /// <param name="connectionString">Connection string to the storage account.</param>
    /// <param name="containerName">The container to work in.</param>
    /// <param name="logger">
    /// Optional. Without it nothing is logged and nothing is allocated for it. Which logger sits
    /// behind the abstraction is the application's decision, this library brings none.
    /// </param>
    protected StorageBase(string connectionString, string containerName, ILogger? logger = null)
    {
        ConnectionString = connectionString;
        ContainerName = containerName;
        Logger = logger ?? NullLogger.Instance;
    }

    public abstract Task<IEnumerable<string>> ListFiles(string? path = null, CancellationToken cancellationToken = default);
    public abstract Task<bool> Exists(string path, CancellationToken cancellationToken = default);
    public abstract Task Delete(string path, CancellationToken cancellationToken = default);
    public abstract Task<Stream> DownloadFile(string path, CancellationToken cancellationToken = default);
    public abstract Task UploadFile(Stream stream, string destinationPath, CancellationToken cancellationToken = default);
}