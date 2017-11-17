using MediaPortal.Data.Repositories;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThumbnailCreatorWebJob.Model;

namespace ThumbnailCreatorWebJob.Listeners
{
    public class ThumbnailListener
    {
        public CloudQueue Queue { get; private set; }

        public CloudBlobContainer BlobContainer { get; set; }

        private readonly FileSystemRepository _fileSystemRepository;


        public ThumbnailListener()
        {
            Queue = GetQueueReference();
            BlobContainer = GetContainerReference();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _fileSystemRepository = new FileSystemRepository(connectionString);

        }

        public void Listen()
        {
            Console.WriteLine("Thumbnail listener started listen:");
            while (true)
            {
                // Get message frim queue; message will be invisible for 5 seconds
                CloudQueueMessage message = Queue.GetMessage(new TimeSpan(0, 0, 10));

                // if there are no more messages
                if (message == null)
                {
                    continue;
                }

                FileIdBlobModel fileIdBlob = JsonConvert.DeserializeObject<FileIdBlobModel>(message.AsString);

                Console.WriteLine(message.AsString);
                var thumbnailLink = CreateThumbnailAsync(fileIdBlob.BlobLink).Result;
                Queue.DeleteMessage(message);
                
                
                UpdateFileSystem(fileIdBlob.FileId, thumbnailLink);
                // some operation here is performed
            }
        }

        private void UpdateFileSystem(int id, string uri)
        {
            var cuttedUri = uri.Replace(ConfigurationManager.AppSettings.Get("azureStorageBlobLink"), "");
            _fileSystemRepository.FileSystemAddThumbnailLink(id, cuttedUri);
        }

        private async Task<string> CreateThumbnailAsync(string blobUri)
        {
            // TODO : PUT THIS CONNECTION STRING TO APP SETTINGS
            string connectionString = ConfigurationManager.ConnectionStrings["azureConnection"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobUri), storageAccount.Credentials);
            var thumbnailImage = await ResizeBlobAsync(blob).ConfigureAwait(false);

            if (thumbnailImage.Length > 0)
            {
                var guidName = Guid.NewGuid().ToString();
                var blobName = guidName + Path.GetExtension(blob.Name);

                CloudBlockBlob blobThumbnail = BlobContainer.GetBlockBlobReference(blobName);
                blobThumbnail.DeleteIfExists();

                await blobThumbnail.UploadFromByteArrayAsync(thumbnailImage, 0, thumbnailImage.Length).ConfigureAwait(false);
                return blobThumbnail.Uri.ToString();
            }

            return null;
        }

        private async Task<byte[]> ResizeBlobAsync(CloudBlockBlob blob)
        {
            byte[] thumbnailImage = new byte[0];
            if (Path.GetExtension(blob.Name).Equals(".png") || Path.GetExtension(blob.Name).Equals(".jpg"))
            {
                var imageStream = await blob.OpenReadAsync();

                using (Image img = Image.FromStream(imageStream))
                {
                    int h = 200;
                    int w = 200;

                    using (Bitmap b = new Bitmap(img, new Size(w, h)))
                    {
                        using (MemoryStream thumbnailStream = new MemoryStream())
                        {
                            b.Save(thumbnailStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            thumbnailImage = thumbnailStream.ToArray();
                        }
                    }
                }
                return thumbnailImage;
            }
            else
            {
                // TODO VideoThumbnail
                return thumbnailImage;
            }
        }

        private CloudBlobContainer GetContainerReference()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=teamresponse200;AccountKey=lk2ZcpWPmltfWQClFaesuTs01+8zSvv1yOm1UsjsHXMBc42OkFc/41jf7P3DGvlwa2EgicYPVFPKs55OPWo4/Q==";
            string ContainerName = "filesystem";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            container.CreateIfNotExists();

            return container;
        }

        private CloudQueue GetQueueReference()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=teamresponse200;AccountKey=lk2ZcpWPmltfWQClFaesuTs01+8zSvv1yOm1UsjsHXMBc42OkFc/41jf7P3DGvlwa2EgicYPVFPKs55OPWo4/Q==";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("filesthumbnailsqueue");
            queue.CreateIfNotExists();

            return queue;
        }


    }
}
