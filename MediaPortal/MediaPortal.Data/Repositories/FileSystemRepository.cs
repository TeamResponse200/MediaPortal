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
using System.Data;
using MediaPortal.Data.Models;

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
                return dbContext.FileSystems.Where(x => x.Id == fileSystemId)
                                            .Include(x => x.Tags)
                                            .ToList().FirstOrDefault();
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
                return dbContext.FileSystems.Where(x => x.UserId == userId)
                                            .Include(x => x.Tags)
                                            .ToList();
            }
        }

        public IEnumerable<FileSystem> GetAll(string userId, int? parentId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.FileSystems.Where(x => x.UserId == userId && x.ParentId == parentId)
                                            .Include(x => x.Tags)
                                            .ToList();
                //return dbContext.FileSystems.Where(x => x.UserId == userId && x.ParentId == parentId).ToList();
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

        public List<FileDeleteModel> SearchFileSystemForDelete(int fileSystemId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                var param = new SqlParameter("@Id", fileSystemId);

                List<FileDeleteModel> fileSystem = dbContext.Database.SqlQuery<FileDeleteModel>("prFileSystemDelete @Id", param).ToList();

                return fileSystem;
            }
        }

        public void DeleteFileSystem(int fileSystemId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                var fileSystem = dbContext.FileSystems.Where(x => x.Id == fileSystemId).FirstOrDefault();

                dbContext.Entry(fileSystem).State = EntityState.Deleted;
                dbContext.SaveChanges();
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

        public void AddTag(int fileSystem, int tag)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                var filesystems = dbContext.FileSystems.Where(x => x.Id == fileSystem).FirstOrDefault();
                var tags = dbContext.Tags.Where(x => x.Id == tag).FirstOrDefault();

                filesystems.Tags.Add(tags);
                dbContext.SaveChanges();
            }
        }
    }
}
