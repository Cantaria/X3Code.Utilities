
// ReSharper disable InconsistentNaming

using System.IO;
using System.Threading.Tasks;
using X3Code.Azure.Utils.Storage;
using Xunit;

namespace X3Code.UnitTests.Azure.Storage
{
    public class Blob_specs
    {
        //Because this test can only run with sensitive data, it will be disabled by default.
        //To run this tests, set up connection string and container of your blob-storage
        //  and never(!) commit these two secrets ;)
        private bool _testDisabled = true;
        
        [Fact]
        public async Task CanUploadFile()
        {
            if (_testDisabled) return;
            
            var storageFilePath = "folder/testfile.txt";
            var localFile = "Azure/testfile.txt";
            
            if (!File.Exists(localFile)) return;
            var stream = File.OpenRead(localFile);

            var connectionString = "";
            var container = "";

            var storage = new AzureBlobStorage(connectionString, container);
            await storage.UploadFile(stream, storageFilePath);
            var exist = await storage.Exists(storageFilePath);
            Assert.True(exist);
            
            var downloadFile = await storage.DownloadFile(storageFilePath);
            Assert.True(downloadFile.Length > 0);

            var files = await storage.ListFiles();
            Assert.Single(files);
            
            storage.Delete(storageFilePath);
            var doesNotExist = await storage.Exists(storageFilePath);
            Assert.False(doesNotExist);
        }
    }
}