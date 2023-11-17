using System.Collections.Generic;
using System.IO;
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

    public abstract Task<IEnumerable<string>> ListFiles(string? path = null);
    public abstract Task<bool> Exists(string path);
    public abstract Task Delete(string path);
    public abstract Task<Stream> DownloadFile(string path);
    public abstract Task UploadFile(Stream stream, string destinationPath);
}