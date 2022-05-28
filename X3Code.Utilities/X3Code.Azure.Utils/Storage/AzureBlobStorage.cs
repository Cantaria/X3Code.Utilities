using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace X3Code.Azure.Utils.Storage
{
    public class AzureBlobStorage : StorageBase
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorage(string connectionString, string containerName) : base(connectionString, containerName)
        {
            _containerClient = new BlobContainerClient(connectionString, containerName);
        }

        /// <summary>
        /// List files from a specific blob path
        /// </summary>
        /// <param name="path">Path to the folder the files should be listed, can be null</param>
        /// <returns></returns>
        public override async Task<IEnumerable<string>> ListFiles(string? path = null)
        {
            var result = new List<string>();
            var blobs = _containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, path);
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
        /// <returns></returns>
        public override async Task<bool> Exists(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            
            var blob = GetBlob(path);
            return await ExistsBlob(blob);
        }

        /// <summary>
        /// Deletes the given file from the storage
        /// </summary>
        /// <param name="path"></param>
        public override async Task Delete(string path)
        {
            var blob = GetBlob(path);
            await blob.DeleteAsync();
        }

        /// <summary>
        /// Downloads a specific file from blob
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<Stream> DownloadFile(string path)
        {
            var blob = GetBlob(path);
            if (!await ExistsBlob(blob)) throw new Exception($"The blob on path [{path}] could not be found.");

            var stream = new MemoryStream();
            await blob.DownloadToAsync(stream);
            return stream;
        }

        /// <summary>
        /// Uploads a specific file to a blob
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public override async Task UploadFile(Stream stream, string destinationPath)
        {
            var destinationBlob = GetBlob(destinationPath);
            if (stream.Length == 0) throw new Exception("Stream is empty.");
            stream.Position = 0;

            await destinationBlob.UploadAsync(stream);
        }

        private BlobClient GetBlob(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            
            return _containerClient.GetBlobClient(path);
        }

        private async Task<bool> ExistsBlob(BlobClient blob)
        {
            return await blob.ExistsAsync();
        }
    }
}