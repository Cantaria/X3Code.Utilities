
// ReSharper disable InconsistentNaming

using System;
using System.IO;
using System.Linq;
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
        public void CanUploadFile()
        {
            if (_testDisabled) return;
            
            var storageFilePath = "folder/testfile.txt";
            var localFile = "Azure/testfile.txt";
            
            if (!File.Exists(localFile)) return;
            var stream = File.OpenRead(localFile);

            var connectionString = "";
            var container = "";

            var storage = new AzureBlobStorage(connectionString, container);
            storage.UploadFile(stream, storageFilePath).GetAwaiter().GetResult();
            var exist = storage.Exists(storageFilePath).GetAwaiter().GetResult();
            Assert.True(exist);
            
            var downloadFile = storage.DownloadFile(storageFilePath).GetAwaiter().GetResult();
            Assert.True(downloadFile.Length > 0);

            var files = storage.ListFiles().GetAwaiter().GetResult();
            Assert.Single(files);
            
            storage.Delete(storageFilePath).GetAwaiter().GetResult();
            var doesNotExist = storage.Exists(storageFilePath).GetAwaiter().GetResult();
            Assert.False(doesNotExist);
        }
    }
}