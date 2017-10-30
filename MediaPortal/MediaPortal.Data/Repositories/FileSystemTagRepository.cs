using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Data.DataAccess;
using MediaPortal.Data.Interface;
using MediaPortal.Data.EntitiesModel;

namespace MediaPortal.Data.Repositories
{
    public class FileSystemTagRepository : IRepository<FileSystemTag>
    {
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        public FileSystemTagRepository(string connectionString)
        {
            _mediaPortalDbContext = new MediaPortalDbContext(connectionString);
        }

        public FileSystemTag Get(int id)
        {
            return null;
        }

        public IEnumerable<FileSystemTag> GetAll()
        {
            return null;
        }
    }
}
