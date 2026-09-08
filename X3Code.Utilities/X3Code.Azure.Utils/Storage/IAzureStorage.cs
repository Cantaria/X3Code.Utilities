using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace X3Code.Azure.Utils.Storage;

public interface IAzureStorage
{
    /// <summary>
    /// List files from a specific blob path
    /// </summary>
    /// <param name="path">Path to the folder the files should be listed, can be null</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<string>> ListFiles(string? path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the blob exists
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns></returns>
    Task<bool> Exists(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the given file from the storage
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    Task Delete(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a specific file from blob
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    Task<Stream> DownloadFile(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a specific file to a blob
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="destinationPath"></param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns></returns>
    Task UploadFile(Stream stream, string destinationPath, CancellationToken cancellationToken = default);
}