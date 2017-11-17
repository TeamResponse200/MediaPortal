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

        Task<Tuple<byte[], string>> DownloadFileSystem(int fileSystemId);

        Tuple<byte[], string> DownloadFileSystemZIP(int[] fileSystemsId);

        void RenameFileSystem(int fileSystemId, string name);

        void AddTag(string fileSystemId, string tagValue);

        void UploadAndInsertFiles(FilesToUploadDTO filesToUpload);

        Task<Stream> GetFileSystemThumbnailAsync(int fileSystemId);
    }
}
