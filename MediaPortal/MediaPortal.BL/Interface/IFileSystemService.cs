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

        void InsertFileSystem(FileSystemDTO model);
    }
}
