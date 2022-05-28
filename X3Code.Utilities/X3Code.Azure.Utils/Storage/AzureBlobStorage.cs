using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace X3Code.Azure.Utils.Storage
{
    public class AzureBlobStorage
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorage(string connectionString, string containerName)
        {
            _containerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<IEnumerable<string>> ListFiles(string path)
        {
            return new List<string>();
        }
    }
}