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
    public class FileSystemRepository : IFileSystemRepository<FileSystem>
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
                return dbContext.FileSystems.ToList();
            }
        }

        public IEnumerable<FileSystem> GetAll(string userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystems.Where(x => x.UserId == userId).ToList();
            }
        }

        public IEnumerable<FileSystem> GetAll(string userId, int? parentId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystems.Where(x => x.UserId == userId && x.ParentId == parentId).ToList();
            }
        }

        public void InsertObject(FileSystem file)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                dbContext.FileSystems.Add(file);

                dbContext.SaveChanges();
            }
        }

        public void DeleteFileSystem(int fileSystemId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                var fileSystem = dbContext.FileSystems.Where(x => x.Id == fileSystemId).FirstOrDefault();

                if (fileSystem != null)
                {
                    dbContext.FileSystems.Remove(fileSystem);
                    dbContext.SaveChanges();
                }
            }
        }

        public void RenameFileSystem(int fileSystemId, string name)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                var fileSystem = dbContext.FileSystems.Where(x => x.Id == fileSystemId).FirstOrDefault();

                if (fileSystem != null)
                {
                    fileSystem.Name = name;
                    dbContext.SaveChanges();
                }
            }
        }

        public void AddTag(int fileSystemId, List<Tag> tags)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                var fileSystem = dbContext.FileSystems.Where(x => x.Id == fileSystemId).FirstOrDefault();

                if (fileSystem != null)
                {
                    foreach (var tag in tags)
                    {
                        fileSystem.Tags.Add(tag);
                    }

                    dbContext.SaveChanges();
                }
            }
        }
    }
}
