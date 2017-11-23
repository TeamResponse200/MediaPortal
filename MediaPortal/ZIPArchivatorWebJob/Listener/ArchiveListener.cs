using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MediaPortal.BL.Interface;
using MediaPortal.BL.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZIPArchivatorWebJob.Model;

namespace ZIPArchivatorWebJob.Listener
{
    public class ArchiveListener
    {
        public CloudQueue Queue { get; private set; }

        public CloudBlobContainer BlobContainer { get; set; }

        private IFileSystemService _fileSystemService;

        public ArchiveListener(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;

            string dbConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string azureConnectionString = ConfigurationManager.ConnectionStrings["azureConnection"].ConnectionString;

            Queue = GetQueueReference(azureConnectionString);
            BlobContainer = GetContainerReference(azureConnectionString);

        }

        private CloudQueue GetQueueReference(string azureConnectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("ziparchivequeue");
            queue.CreateIfNotExists();

            return queue;
        }

        private CloudBlobContainer GetContainerReference(string azureConnectionString)
        {
            string ContainerName = "filesystem";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            container.CreateIfNotExists();

            return container;
        }

        public async Task Listen()
        {
            Console.WriteLine("Archive listener started listen:");
            while (true)
            {
                var messages = Queue.GetMessages(32);

                Parallel.ForEach(messages, async message =>
                {

                    ArchiveModel archiveModel = JsonConvert.DeserializeObject<ArchiveModel>(message.AsString);
                    Console.WriteLine(message.AsString);

                    byte[] outputZIP;

                    using (MemoryStream outputMemStream = new MemoryStream())
                    {
                        using (ZipOutputStream zipStream = new ZipOutputStream(outputMemStream))
                        {
                            zipStream.SetLevel(3);

                            string entryLocateName = "";

                            foreach (var fileSystemId in archiveModel.FileSystemsId)
                            {
                                FileSystemDTO fileSystem = _fileSystemService.Get(fileSystemId);
                                if(fileSystem != null)
                                {
                                    await ZipArchivingFileSystemTree(fileSystem, zipStream, archiveModel.UserId, entryLocateName);
                                }                                
                            }

                            zipStream.IsStreamOwner = false;
                            zipStream.Close();

                            outputMemStream.Position = 0;

                            outputZIP = outputMemStream.ToArray();
                        }
                    }

                    var archivelLink = await UploadFileInBlocksAsync(outputZIP, archiveModel.Id);

                    Queue.DeleteMessage(message);

                });
                              
            }
        }

        public async Task ZipArchivingFileSystemTree(FileSystemDTO fileSystem, ZipOutputStream zipStream, string userId, string entryLocateName)
        {
            if (fileSystem.BlobLink != null)
            {
                string blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobLink;
                byte[] fileBytes = await DownloadFile(blobLink);

                string locateName = entryLocateName;

                if (entryLocateName != null)
                {
                    locateName += fileSystem.Name + fileSystem.Type;
                }

                string entryName = ZipEntry.CleanName(locateName);

                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.Size = (long)fileSystem.Size;
                newEntry.IsUnicodeText = true;

                zipStream.PutNextEntry(newEntry);

                using (MemoryStream inputMemoryStream = new MemoryStream(fileBytes))
                {
                    byte[] buffer = new byte[4096];
                    StreamUtils.Copy(inputMemoryStream, zipStream, buffer);
                }

                zipStream.CloseEntry();
            }
            else
            {
                string locateName = entryLocateName;

                locateName += fileSystem.Name + "/";

                string entryName = ZipEntry.CleanName(locateName);
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.IsUnicodeText = true;

                zipStream.PutNextEntry(newEntry);
                zipStream.CloseEntry();

                List<FileSystemDTO> fileSystems = _fileSystemService.GetAll(userId, fileSystem.Id).ToList();

                foreach (var fs in fileSystems)
                {
                    await ZipArchivingFileSystemTree(fs, zipStream, userId, locateName);
                }
            }
        }

        public async Task<byte[]> DownloadFile(string blobLink)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["azureConnection"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobLink), storageAccount.Credentials);            

            blob.FetchAttributes();
            long fileSize = blob.Properties.Length;

            var blobContents = new byte[fileSize];

            await blob.DownloadToByteArrayAsync(blobContents, 0);

            return blobContents;
        }

        public async Task<string> UploadFileInBlocksAsync(byte[] file, string guid)
        {  
            var guidName = guid;
            var blobName = guidName + ".zip";

            CloudBlockBlob blobArchive = BlobContainer.GetBlockBlobReference(blobName);
            blobArchive.DeleteIfExists();

            await blobArchive.UploadFromByteArrayAsync(file, 0, file.Length);

            var ur = blobArchive.Uri.ToString();

            return ur;
        }
    }
}
