using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace X3Code.Azure.Utils.Storage;

/// <summary>
/// Base class for all Azure Storage types
/// </summary>
public abstract class StorageBase : IAzureStorage
{
    protected string ConnectionString { get; }
    protected string ContainerName { get; }

    protected StorageBase(string connectionString, string containerName)
    {
        ConnectionString = connectionString;
        ContainerName = containerName;
    }

    public abstract Task<IEnumerable<string>> ListFiles(string? path = null, CancellationToken cancellationToken = default);
    public abstract Task<bool> Exists(string path, CancellationToken cancellationToken = default);
    public abstract Task Delete(string path, CancellationToken cancellationToken = default);
    public abstract Task<Stream> DownloadFile(string path, CancellationToken cancellationToken = default);
    public abstract Task UploadFile(Stream stream, string destinationPath, CancellationToken cancellationToken = default);
}