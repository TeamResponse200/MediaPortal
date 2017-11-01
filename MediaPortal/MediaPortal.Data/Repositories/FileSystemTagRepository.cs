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
        private string _connectionString;

        public FileSystemTagRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public FileSystemTag Get(string userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystemTags.Find(userId);
            }
        }

        public IEnumerable<FileSystemTag> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystemTags;
            }
        }

        public IEnumerable<FileSystemTag> GetAll(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileSystemTag> GetAll(string userId, string fileSystemParentId)
        {
            throw new NotImplementedException();
        }
    }
}
