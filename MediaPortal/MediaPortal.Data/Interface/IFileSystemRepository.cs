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

        void InsertObject(T t);
    }
}
