using MediaPortal.BL.Models;
using System;
using System.Collections.Generic;
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

        void DeleteFileSystem(int[] fileSystemsId);

        void RenameFileSystem(int fileSystemId, string name);

        void AddTag(int fileSystemId, string tegName);
    }
}
