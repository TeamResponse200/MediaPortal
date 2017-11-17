using MediaPortal.Data.EntitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Interface
{
    public interface IFileSystemRepository<T> : IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string userId);

        IEnumerable<T> GetAll(string userId, int? fileSystemParentId);

        int InsertObject(T t);

        void DeleteFileSystem(int fileSystemId);

        void RenameFileSystem(int fileSystemId, string name);

        void FileSystemAddThumbnailLink(int fileSystemId, string link);

        void AddTag(int fileSystem, int tag);
    }
}
