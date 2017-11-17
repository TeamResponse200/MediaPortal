using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.DataAccess;
using MediaPortal.Data.Interface;
using System.Data.Entity;
using System.Data.SqlClient;

namespace MediaPortal.Data.Repositories
{
    public class FileSystemRepository : IFileSystemRepository<FileSystem>
    {
        private string _connectionString;

        public FileSystemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public FileSystem Get(int fileSystemId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystems.Where(x => x.Id == fileSystemId).FirstOrDefault();
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

        public int InsertObject(FileSystem file)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                dbContext.FileSystems.Add(file);

                dbContext.SaveChanges();

                return file.Id;
            }
        }

        public void DeleteFileSystem(int fileSystemId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                var param = new SqlParameter("@Id", fileSystemId);

                //var fileSystem = dbContext.FileSystems.SqlQuery("prFileSystemDelete @Id", param).ToList();

                //dbContext.Database.ExecuteSqlCommand("WITH [CTE](Id, ParentId) AS (         SELECT Id, ParentId         FROM FileSystem fs         WHERE (fs.Id = @Id)          UNION ALL          SELECT fs.Id, fs.ParentId             FROM FileSystem fs INNER JOIN [CTE] p ON fs.ParentId = p.Id      ) DELETE FROM FileSystem WHERE Id IN(     SELECT TOP 100 PERCENT t.Id     FROM [CTE] t)", param);
                dbContext.Database.ExecuteSqlCommand("prFileSystemDelete @Id", param);

                //dbContext.SaveChanges();
                                
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

        public void FileSystemAddThumbnailLink(int fileSystemId, string link)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                var fileSystem = dbContext.FileSystems.Where(x => x.Id == fileSystemId).FirstOrDefault();

                if (fileSystem != null)
                {
                    fileSystem.BlobThumbnail = link;
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
