using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.DataAccess;
using MediaPortal.Data.Interface;

namespace MediaPortal.Data.Repositories
{
    public class FileSystemRepository : IRepository<FileSystem>
    {
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        public FileSystemRepository(MediaPortalDbContext connectionString)
        {
            _mediaPortalDbContext = connectionString;                        
        }

        public FileSystem Get(int id)
        {
            return _mediaPortalDbContext.FileSystems.Find(id);
        }

        public IEnumerable<FileSystem> GetAll()
        {
            return _mediaPortalDbContext.FileSystems;
        }
    }
}
