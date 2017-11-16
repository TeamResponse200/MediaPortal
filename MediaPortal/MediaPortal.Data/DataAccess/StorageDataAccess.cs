﻿using System;
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

        public byte[] DownloadFileInBlocks(string fileName)
        {
            CloudBlobContainer cloudBlobContainer = GetContainerReference();
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(Path.GetFileName(fileName));

            int blockSize = 1024 * 1024; // 1 MB block size

            blob.FetchAttributes();
            long fileSize = blob.Properties.Length;

            var blobContents = new byte[fileSize];
            var fullSizeCount = (int)(fileSize / blockSize);
            var restSize = (int)(fileSize - fullSizeCount * blockSize);

            IEnumerable<int> parts = Enumerable.Range(0, fullSizeCount);
            int currentPart = -1;

            Parallel.ForEach(parts, part =>
            {
                blob.DownloadRangeToByteArray(blobContents, Interlocked.Add(ref currentPart, blockSize), currentPart, blockSize);
            });

            int finalIndexAndOffset = fullSizeCount + restSize;
            blob.DownloadRangeToByteArray(blobContents, finalIndexAndOffset, finalIndexAndOffset, restSize);

            return blobContents;
        }

        #endregion


        public void DeleteFileSystem(string fileName)
        {
            CloudBlobContainer cloudBlobContainer = GetContainerReference();
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(Path.GetFileName(fileName));

            blob.DeleteIfExists();
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


    }
}
