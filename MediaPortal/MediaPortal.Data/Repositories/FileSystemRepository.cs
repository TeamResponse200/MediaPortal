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
        private string _connectionString;

        public FileSystemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public FileSystem Get(string userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystems.Find(userId);
            }
        }

        public IEnumerable<FileSystem> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystems;
            }
        }

        public IEnumerable<FileSystem> GetAll(string userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystems.Where(x => x.UserId == userId);
            }
        }

        public IEnumerable<FileSystem> GetAll(string userId, string parentId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystems.Where(x => x.UserId == userId && x.ParentId == parentId);
            }
        }
    }
}
