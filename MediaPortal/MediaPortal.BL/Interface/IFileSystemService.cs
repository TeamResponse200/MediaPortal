using MediaPortal.BL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.BL.Interface
{
    public interface IFileSystemService
    {
        IEnumerable<FileSystemDTO> GetAllUserFileSystem(string userId);

        IEnumerable<FileSystemDTO> GetUserFileSystem(string userId, int? fileSystemParentId = null);

        Tuple<List<int?>, List<string>> GetFoldersIdNamePair(int? folderID, string userId);

        void InsertFileSystem(FileSystemDTO model);

        Task<bool> DeleteFileSystem(int[] fileSystemsId);

        Task<byte[]> DownloadProcessZIP(string fileSystemName);

        Task<Tuple<byte[], string>> DownloadFileSystem(int fileSystemId);

        string DownloadFileSystemZIP(List<int> fileSystemsId, string userId);

        FileSystemDTO Get(int fileSystemId);

        Task<byte[]> DownloadFile(string blobLink);

        IEnumerable<FileSystemDTO> GetAll(string userId, int fileSystemId);

        Task UploadFileInBlocksAsync(byte[] file, string guidName);

        void RenameFileSystem(int fileSystemId, string name);

        void AddTag(int[] fileSystemId, string tagValue);

        void UploadAndInsertFiles(FilesToUploadDTO filesToUpload);

        Task<Stream> GetFileSystemThumbnailAsync(int fileSystemId);

        void UpdateThumbnail(int id, string thumbnailUri);
    }
}
