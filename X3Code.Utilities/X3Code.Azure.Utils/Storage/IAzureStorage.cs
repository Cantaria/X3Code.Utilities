using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace X3Code.Azure.Utils.Storage
{
    public interface IAzureStorage
    {
        /// <summary>
        /// List files from a specific blob path
        /// </summary>
        /// <param name="path">Path to the folder the files should be listed, can be null</param>
        /// <returns></returns>
        Task<IEnumerable<string>> ListFiles(string? path);

        /// <summary>
        /// Checks if the blob exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<bool> Exists(string path);

        /// <summary>
        /// Deletes the given file from the storage
        /// </summary>
        /// <param name="path"></param>
        Task Delete(string path);

        /// <summary>
        /// Downloads a specific file from blob
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task<Stream> DownloadFile(string path);

        /// <summary>
        /// Uploads a specific file to a blob
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        Task UploadFile(Stream stream, string destinationPath);
    }
}