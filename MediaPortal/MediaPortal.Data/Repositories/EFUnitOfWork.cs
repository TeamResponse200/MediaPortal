using MediaPortal.Data.DataAccess;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        private bool disposed = false;

        private AspNetRoleRepositories _aspNetRoleRepositories;
        private AspNetUserLoginRepositories _aspNetUserLoginRepositories;
        private AspNetUserRepositories _aspNetUserRepositories;
        private FileSystemRepository _fileSyatemRepository;
        private FileSystemTagRepository _fileSystemTagRepository;
        private TagRepository _tagRepository;
        private SharedAccessRepository _sharedAccessRepository;

        public EFUnitOfWork(string connectionString)
        {
            _mediaPortalDbContext = new MediaPortalDbContext(connectionString);
        }

        public IRepository<AspNetUser> AspNetUsers
        {
            get
            {
                if (_aspNetUserRepositories == null)
                    _aspNetUserRepositories = new AspNetUserRepositories(_mediaPortalDbContext);
                return _aspNetUserRepositories;
            }
        }

        public IRepository<AspNetRole> AspNetRoles
        {
            get
            {
                if (_aspNetRoleRepositories == null)
                    _aspNetRoleRepositories = new AspNetRoleRepositories(_mediaPortalDbContext);
                return _aspNetRoleRepositories;
            }
        }

        public IRepository<AspNetUserLogin> AspNetUserLogins
        {
            get
            {
                if (_aspNetUserLoginRepositories == null)
                    _aspNetUserLoginRepositories = new AspNetUserLoginRepositories(_mediaPortalDbContext);
                return _aspNetUserLoginRepositories;
            }
        }

        public IRepository<FileSystem> FileSystems
        {
            get
            {
                if (_fileSyatemRepository == null)
                    _fileSyatemRepository = new FileSystemRepository(_mediaPortalDbContext);
                return _fileSyatemRepository;
            }
        }

        public IRepository<FileSystemTag> FileSystemTags
        {
            get
            {
                if (_fileSystemTagRepository == null)
                    _fileSystemTagRepository = new FileSystemTagRepository(_mediaPortalDbContext);
                return _fileSystemTagRepository;
            }
        }

        public IRepository<SharedAccess> SharedAccesses
        {
            get
            {
                if (_sharedAccessRepository == null)
                    _sharedAccessRepository = new SharedAccessRepository(_mediaPortalDbContext);
                return _sharedAccessRepository;
            }
        }

        public IRepository<Tag> Tags
        {
            get
            {
                if (_tagRepository == null)
                    _tagRepository = new TagRepository(_mediaPortalDbContext);
                return _tagRepository;
            }
        }

        public void Save()
        {
            _mediaPortalDbContext.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _mediaPortalDbContext.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
