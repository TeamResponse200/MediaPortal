using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using MediaPortal.Data.Models;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace MediaPortal.Data.DataAccess
{
    public class StorageDataAccess
    {
        private const string ConnectionStringSettingName = "teamresponse200_AzureStorageConnectionString";
        private const string ContainerName = "filesystem";

        #region Upload

        public void Upload(byte[] file, string fileName)
        {
            UploadFileInBlocks(file, fileName);
        }

        public async Task UploadFileInBlocksAsync(byte[] file, string guid)
        {
            CloudBlobContainer cloudBlobContainer = GetContainerReference();
            //var fileExtension = Path.GetExtension(file.FileName);
            var guidName = guid;
            //var blobName = guidName + fileExtension;
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(guidName);

            blob.DeleteIfExists();
            //await blob.UploadFromStreamAsync(file.InputStream).ConfigureAwait(false);

            using (var stream = new MemoryStream(file, writable: false))
            {
                await blob.UploadFromStreamAsync(stream);
            }
        }

        public async Task<string> UploadFileInBlocksAsync(HttpPostedFileBase file)
        {
            CloudBlobContainer cloudBlobContainer = GetContainerReference();
            var fileExtension = Path.GetExtension(file.FileName);
            var guidName = Guid.NewGuid().ToString();
            var blobName = guidName + fileExtension;
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(blobName);

            blob.DeleteIfExists();
            await blob.UploadFromStreamAsync(file.InputStream).ConfigureAwait(false);
           
            return blob.Uri.ToString();
        }

        public async Task<Stream> GetImageThumbnail(string blobLink)
        {
            string connectionString = CloudConfigurationManager.GetSetting(ConnectionStringSettingName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobLink), storageAccount.Credentials);
            var file = await blob.OpenReadAsync();
            return file;
        }

        public void UploadFileInBlocks(byte[] file, string fileName)
        {
            CloudBlobContainer cloudBlobContainer = GetContainerReference();
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(Path.GetFileName(fileName));

            blob.DeleteIfExists();

            List<string> blockIDs = new List<string>();

            int blockSize = 5 * 1024 * 1024;
            long fileSize = file.Length;

            int fullSizeCount = (int)(fileSize / blockSize);
            int restSize = (int)(fileSize - fullSizeCount * blockSize);

            var blocksById = new Dictionary<int, byte[]>();

            Action<int, int> createBlocks = (currentBlockSize, partId) =>
            {
                byte[] bytesToUpload = new byte[currentBlockSize];
                Array.Copy(file, partId * blockSize, bytesToUpload, 0, bytesToUpload.Length);
                lock (this)
                {
                    blocksById.Add(partId, bytesToUpload);
                }
            };

            Parallel.For(0, fullSizeCount, partId =>
            {
                createBlocks(blockSize, partId);
            });

            createBlocks(restSize, fullSizeCount);

            var blockIds = new ConcurrentBag<string>();

            Parallel.ForEach(blocksById, blockById =>
            {
                string encoded = GetBase64BlockId(blockById.Key);
                blockIds.Add(encoded);
                using (MemoryStream memoryStream = new MemoryStream(blockById.Value, 0, blockById.Value.Length))
                {
                    blob.PutBlock(encoded, memoryStream, null, null, new BlobRequestOptions
                    {
                        RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 1)
                    });
                }
            });

            blob.PutBlockList(blockIds);
        }

        private string GetBase64BlockId(int blockId)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}", blockId.ToString("0000000"))));
        }

        #endregion

        public async Task<byte[]> DownloadFile(string blobLink)
        {
            string connectionString = CloudConfigurationManager.GetSetting(ConnectionStringSettingName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobLink), storageAccount.Credentials);
            
            if (blob.Exists())
            {
                blob.FetchAttributes();
                long fileSize = blob.Properties.Length;

                var blobContents = new byte[fileSize];

                await blob.DownloadToByteArrayAsync(blobContents, 0);

                return blobContents;
            }

            return null;
        }

        public bool IsExistArchive(string blobLink)
        {
            string connectionString = CloudConfigurationManager.GetSetting(ConnectionStringSettingName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobLink), storageAccount.Credentials);

            return blob.Exists();
        }


            public async Task<bool> DeleteFileSystem(string blobLink)
        {
            string connectionString = CloudConfigurationManager.GetSetting(ConnectionStringSettingName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobLink), storageAccount.Credentials);

            return await blob.DeleteIfExistsAsync();
        }

        public CloudBlobContainer GetContainerReference()
        {
            string connectionString = CloudConfigurationManager.GetSetting(ConnectionStringSettingName);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            container.CreateIfNotExists();

            return container;
        }

        public CloudQueue GetQueueReference(string queueName)
        {
            string connectionString = CloudConfigurationManager.GetSetting(ConnectionStringSettingName);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();

            return queue;
        }

        public void PutMessageRequestForThumbnail(int id, string blobUri)
        {
            var queueName = "filesthumbnailsqueue";
            var queue = GetQueueReference(queueName);

            FileIdBlobModel file = new FileIdBlobModel() { FileId = id, BlobLink = blobUri };
            
            string fileJson = JsonConvert.SerializeObject(file, Formatting.Indented);
            CloudQueueMessage messageBlobFile = new CloudQueueMessage(fileJson);

            queue.AddMessage(messageBlobFile);
        }

        public void PutMessageRequestForZIPArchivator(string ZIPId, List<int> fileSystemsId, string userId)
        {
            var queueName = "ziparchivequeue";
            var queue = GetQueueReference(queueName);

            var file = new ArchiveModel() { Id = ZIPId, UserId = userId, FileSystemsId = fileSystemsId };

            string fileJson = JsonConvert.SerializeObject(file, Formatting.Indented);

            CloudQueueMessage messageBlobFile = new CloudQueueMessage(fileJson);

            queue.AddMessage(messageBlobFile);
        }
    }
}
