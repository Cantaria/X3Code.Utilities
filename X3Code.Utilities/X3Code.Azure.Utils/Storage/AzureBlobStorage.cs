using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;

namespace X3Code.Azure.Utils.Storage;

/// <summary>
/// Provides an encapsulation of the basic azure blob storage operations.
/// </summary>
public class AzureBlobStorage : StorageBase
{
    private readonly BlobContainerClient _containerClient;

    /// <inheritdoc/>>
    public AzureBlobStorage(string connectionString, string containerName, ILogger? logger = null) : base(connectionString, containerName, logger)
    {
        _containerClient = new BlobContainerClient(connectionString, containerName);
    }

    /// <summary>
    /// List files from a specific blob path
    /// </summary>
    /// <param name="path">Path to the folder the files should be listed, can be null</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<IEnumerable<string>> ListFiles(string? path = null, CancellationToken cancellationToken = default)
    {
        var result = new List<string>();
        var blobs = _containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, path, cancellationToken);
        await foreach (var blobItem in blobs)
        {
            result.Add(blobItem.Name);
        }

        return result;
    }

    /// <summary>
    /// Checks if the blob exists
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns></returns>
    public override async Task<bool> Exists(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
            
        var blob = GetBlob(path);
        return await ExistsBlob(blob, cancellationToken);
    }

    /// <summary>
    /// Deletes the given file from the blob storage
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    public override async Task Delete(string path, CancellationToken cancellationToken = default)
    {
        var blob = GetBlob(path);
        await blob.DeleteAsync(cancellationToken: cancellationToken);

        StorageLog.Deleted(Logger, path, ContainerName);
    }

    /// <summary>
    /// Downloads a specific blob
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task<Stream> DownloadFile(string path, CancellationToken cancellationToken = default)
    {
        var blob = GetBlob(path);
        if (!await ExistsBlob(blob, cancellationToken))
        {
            StorageLog.NotFound(Logger, path, ContainerName);

            throw new FileNotFoundException($"The blob on path [{path}] could not be found.", path);
        }

        var stream = new MemoryStream();
        await blob.DownloadToAsync(stream, cancellationToken);
        stream.Position = 0; // sonst steht der Stream am Ende und der Aufrufer liest 0 Bytes

        StorageLog.Downloaded(Logger, path, ContainerName, stream.Length);

        return stream;
    }

    /// <summary>
    /// Uploads a specific file to a blob
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="destinationPath"></param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns></returns>
    public override async Task UploadFile(Stream stream, string destinationPath, CancellationToken cancellationToken = default)
    {
        var destinationBlob = GetBlob(destinationPath);
        // Netzwerk- und Kompressionsstreams können weder ihre Länge nennen noch zurückspulen.
        if (stream.CanSeek)
        {
            if (stream.Length == 0) throw new ArgumentException("Stream is empty.", nameof(stream));
            stream.Position = 0;
        }

        await destinationBlob.UploadAsync(stream, cancellationToken);

        StorageLog.Uploaded(Logger, destinationPath, ContainerName);
    }

    #region Private Helpers

    private BlobClient GetBlob(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            
        return _containerClient.GetBlobClient(path);
    }

    private static async Task<bool> ExistsBlob(BlobClient blob, CancellationToken cancellationToken)
    {
        return await blob.ExistsAsync(cancellationToken);
    }

    #endregion
}