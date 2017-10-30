using MediaPortal.Data.EntitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<AspNetRole> AspNetRoles { get; }
        IRepository<AspNetUser> AspNetUsers { get; }
        IRepository<AspNetUserLogin> AspNetUserLogins { get; }
        IRepository<FileSystem> FileSystems { get; }
        IRepository<FileSystemTag> FileSystemTags { get; }
        IRepository<SharedAccess> SharedAccesses { get; }
        IRepository<Tag> Tags { get; }

        void Save();
    }
}
