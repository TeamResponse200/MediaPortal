using MediaPortal.Data.Interface;
using MediaPortal.Data.Repositories;
using MediaPortal.Data;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.BL.Interface;
using MediaPortal.BL.Models;
using MediaPortal.Data.EntitiesModel;
using AutoMapper;
using MediaPortal.BL.Infrastructure;
using System.Diagnostics;
using System.Data;
using MediaPortal.Data.DataAccess;
using System.Web;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace MediaPortal.BL.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly IFileSystemRepository<FileSystem> _fileSystemRepository;

        private readonly ITagRepository<Tag> _tagRepository;

        private readonly StorageDataAccess _storageDataAccess;

        public FileSystemService(IFileSystemRepository<FileSystem> fileSyatemRepository, ITagRepository<Tag> tagRepository)
        {
            _fileSystemRepository = fileSyatemRepository;
            _tagRepository = tagRepository;

            _storageDataAccess = new StorageDataAccess();
        }


        // add blob 
        //public void InsertFile(FileSystemDTO model, byte[] file, string fileName)
        //{
        //    Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystem>());
        //    var fileSystem = Mapper.Map<FileSystem>(model);

        //    _storageDataAccess.Upload(file, fileName);

        //    if (fileSystem != null)
        //    {
        //        _fileSystemRepository.InsertObject(fileSystem);
        //    }

        //}

        public IEnumerable<FileSystemDTO> GetAllUserFileSystem(string userId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));

            var fileSystems = _fileSystemRepository.GetAll(userId);

            return Mapper.Map<IEnumerable<FileSystem>, List<FileSystemDTO>>(fileSystems);
        }

        public IEnumerable<FileSystemDTO> GetUserFileSystem(string userId, int? fileSystemParentId = null)
        {
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<FileSystem, FileSystemDTO>();
            //    cfg.CreateMap<Tag, TagDTO>();
            //});

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));

            IEnumerable<FileSystem> fileSystem = null;
            try
            {
                fileSystem = _fileSystemRepository.GetAll(userId, fileSystemParentId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
            return Mapper.Map<IEnumerable<FileSystem>, IEnumerable<FileSystemDTO>>(fileSystem);
        }

        public void InsertFileSystem(FileSystemDTO model)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystem>());

            var fileSystem = Mapper.Map<FileSystem>(model);

            try
            {
                _fileSystemRepository.InsertObject(fileSystem);
            }
            catch (DataException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public Tuple<List<int?>, List<string>> GetFoldersIdNamePair(int? folderID, string userId)
        {
            var folderIDs = new List<int?>();
            var folderNames = new List<string>();

            IEnumerable<FileSystemDTO> allFiles = new List<FileSystemDTO>();

            folderIDs.Add(folderID);

            try
            {
                allFiles = GetAllUserFileSystem(userId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }

            while (folderID != null)
            {
                var parent = allFiles.Where(file => file.Id == folderID).FirstOrDefault();

                if (parent != null)
                {
                    var parentID = parent.ParentId;

                    if (parentID != null)
                    {
                        folderIDs.Add(parentID);
                    }
                    folderID = parentID;
                }
            }

            foreach (int? id in folderIDs)
            {
                var name = allFiles.Where(file => file.Id == id).FirstOrDefault().Name;
                folderNames.Add(name);
            }

            folderIDs.Reverse();
            folderNames.Reverse();

            return new Tuple<List<int?>, List<string>>(folderIDs, folderNames);
        }

        public async Task<bool> DeleteFileSystem(int[] fileSystemsId)
        {
            try
            {
                foreach (var fileSystemId in fileSystemsId)
                {
                    var FileSystemsForDelete = _fileSystemRepository.SearchFileSystemForDelete(fileSystemId);

                    if (FileSystemsForDelete != null)
                    {
                        FileSystemsForDelete.Reverse();

                        foreach (var fileSystem in FileSystemsForDelete)
                        {
                            _fileSystemRepository.DeleteFileSystem(fileSystem.Id);

                            if (fileSystem.BlobLink != null)
                            {
                                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobLink;
                                await _storageDataAccess.DeleteFileSystem(blobLink);

                                if (fileSystem.BlobThumbnail != null)
                                {
                                    var blobThumbnailLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobThumbnail;
                                    await _storageDataAccess.DeleteFileSystem(blobThumbnailLink);
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }
        }

        public async Task<byte[]> DownloadProcessZIP(string fileSystemName)
        {
            byte[] fileBytes;

            try
            {
                string containerName = ConfigurationManager.AppSettings.Get("containerNameAzureStorageBlob");
                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + containerName + fileSystemName + ".zip";

                fileBytes = await _storageDataAccess.DownloadFile(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }

            return fileBytes;
        }

        public bool IsExistArchive(string fileSystemName)
        {
            bool existArchive = false;

            try
            {
                string containerName = ConfigurationManager.AppSettings.Get("containerNameAzureStorageBlob");
                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + containerName + fileSystemName + ".zip";

                existArchive = _storageDataAccess.IsExistArchive(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }

            return existArchive;
        }

        public async Task DeleteFileSystemByName(string fileSystemName)
        {
            try
            {
                string containerName = ConfigurationManager.AppSettings.Get("containerNameAzureStorageBlob");
                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + containerName + fileSystemName + ".zip";

                await _storageDataAccess.DeleteFileSystemByName(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }            
        }

        public async Task<Tuple<byte[], string>> DownloadFileSystem(int fileSystemId)
        {
            byte[] fileBytes;
            string fileType;

            try
            {
                var fileSystem = _fileSystemRepository.Get(fileSystemId);

                fileType = fileSystem.Type;

                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobLink;

                fileBytes = await _storageDataAccess.DownloadFile(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
                throw;
            }

            return new Tuple<byte[], string>(fileBytes, fileType);
        }

        public string DownloadFileSystemZIP(List<int> fileSystemsId, string userId)
        {
            string ZIParchiveId = Guid.NewGuid().ToString();
            
            try
            {
                _storageDataAccess.PutMessageRequestForZIPArchivator(ZIParchiveId, fileSystemsId, userId);                
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }            

            return ZIParchiveId;
        }

        public FileSystemDTO Get(string userId,int? fileSystemId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));
            
            FileSystem fileSystem = null;
            try
            {
                fileSystem = _fileSystemRepository.Get(userId,fileSystemId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);                
            }

            return Mapper.Map<FileSystem, FileSystemDTO>(fileSystem);
        }

        public FileSystemDTO Get(int fileSystemId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));

            FileSystem fileSystem = null;
            try
            {
                fileSystem = _fileSystemRepository.Get(fileSystemId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return Mapper.Map<FileSystem, FileSystemDTO>(fileSystem);
        }

        public async Task<byte[]> DownloadFile(string blobLink)
        {
            byte[] fileBytes = null;

            try
            {
                fileBytes = await _storageDataAccess.DownloadFile(blobLink);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return fileBytes;
        }

        public IEnumerable<FileSystemDTO> GetAll(string userId, int fileSystemId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(to => to.Tags, opt => opt.MapFrom(from => from.Tags.Select(o => new TagDTO { Id = o.Id, Name = o.Name }).ToList())));

            IEnumerable<FileSystem> fileSystem = null;

            try
            {
                fileSystem = _fileSystemRepository.GetAll(userId, fileSystemId);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return Mapper.Map<IEnumerable<FileSystem>, IEnumerable<FileSystemDTO>>(fileSystem);
        }

        public async Task ZipArchivingFileSystemTree(FileSystem fileSystem, ZipOutputStream zipStream, string userId, string entryLocateName)
        {  
            if (fileSystem.BlobLink != null)
            {
                var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobLink;
                byte[] fileBytes = await _storageDataAccess.DownloadFile(blobLink);

                string locateName = entryLocateName;

                if (entryLocateName != null)
                {
                    locateName += fileSystem.Name + fileSystem.Type;
                }
                
                string entryName = ZipEntry.CleanName(locateName);

                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.Size = (long)fileSystem.Size;
                newEntry.IsUnicodeText = true;

                zipStream.PutNextEntry(newEntry);

                using (MemoryStream inputMemoryStream = new MemoryStream(fileBytes))
                {
                    byte[] buffer = new byte[4096];
                    StreamUtils.Copy(inputMemoryStream, zipStream, buffer);
                }
                    
                zipStream.CloseEntry();
            }
            else
            {
                string locateName = entryLocateName;
                
                locateName += fileSystem.Name + "/";                

                string entryName = ZipEntry.CleanName(locateName); 
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.IsUnicodeText = true;

                zipStream.PutNextEntry(newEntry);
                zipStream.CloseEntry();

                List<FileSystem> fileSystems = _fileSystemRepository.GetAll(userId, fileSystem.Id).ToList();
                foreach (var fs in fileSystems)
                {
                    await ZipArchivingFileSystemTree(fs, zipStream, userId, locateName);
                }
            }       
        }

        public void RenameFileSystem(int fileSystemId, string name)
        {
            try
            {
                _fileSystemRepository.RenameFileSystem(fileSystemId, name);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public async Task<Stream> GetFileSystemThumbnailAsync(string userID,int fileSystemId)
        {
            var fileSystem = _fileSystemRepository.Get(userID,fileSystemId);
            var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobThumbnail;
            Stream fileStream;
            try
            {
                fileStream = await _storageDataAccess.GetFileStream(blobLink);
                return fileStream;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
            }

            return null;

        }

        public async Task<Stream> GetFileSystemStreamAsync(string userID, int fileSystemId)
        {
            var fileSystem = _fileSystemRepository.Get(userID, fileSystemId);
            var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobLink;
            Stream fileStream;
            try
            {
                fileStream = await _storageDataAccess.GetFileStream(blobLink);
                return fileStream;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
            }

            return null;

        }

        public async Task UploadFileInBlocksAsync(byte[] file, string guidName)
        {
            try
            {
               await _storageDataAccess.UploadFileInBlocksAsync(file, guidName);
            }
            catch
            {

            }
        }

        public void UploadAndInsertFiles(FilesToUploadDTO filesToUpload)
        {
            Parallel.ForEach(filesToUpload.Files, file =>
            {
                if (file != null)
                {
                    var uri = _storageDataAccess.UploadFileInBlocksAsync(file).Result;

                    var cuttedUri = GetFileCuttedUri(uri);

                    var fileSystem = new FileSystem()
                    {
                        UserId = filesToUpload.UserID,
                        ParentId = filesToUpload.ParrentID,
                        Name = Path.GetFileNameWithoutExtension(file.FileName),
                        Type = Path.GetExtension(file.FileName),
                        Size = file.ContentLength,
                        BlobLink = cuttedUri,
                        UploadDate = DateTime.Now,
                        CreationDate = DateTime.Now
                    };

                    //

                    var insertedId = _fileSystemRepository.InsertObject(fileSystem);

                    _storageDataAccess.PutMessageRequestForThumbnail(insertedId, uri);
                }
            });
        }

        private string GetFileCuttedUri(string blobLink)
        {
            var cuttedLink = blobLink.Replace(ConfigurationManager.AppSettings.Get("azureStorageBlobLink"), "");
            return cuttedLink;
        }

        public void AddTag(int[] fileSystemId, string tagValue)
        {
            for (int i = 0; i < fileSystemId.Length; i++)
            {
                var fileSystem = _fileSystemRepository.Get(fileSystemId[i]);

                if (fileSystem != null)
                {
                    var tag = _tagRepository.Get(tagValue);

                    int tagId;

                    if (tag == null)
                    {
                        var tagObj = new Tag()
                        {
                            Name = tagValue,
                        };

                        tagId = _tagRepository.InsertObject(tagObj);

                        var currentTag = _tagRepository.Get(tagId);

                        _fileSystemRepository.AddTag(fileSystemId[i], currentTag.Id);
                    }
                    else
                    {
                        _fileSystemRepository.AddTag(fileSystemId[i], tag.Id);
                    }
                }
            }

        }

        public void MoveFileSystem(List<int> fileSystemsId, int? fileSystemParentId, string userId)
        {
            try
            {
                Parallel.ForEach(fileSystemsId, fileSystemId =>
                {
                    _fileSystemRepository.ChangeFileSystemParentId(fileSystemId, fileSystemParentId, userId);
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public void UpdateThumbnail(int id, string thumbnailUri)
        {
            _fileSystemRepository.FileSystemAddThumbnailLink(id, thumbnailUri);
        }

        public string SetFileReadPermission(string blobLink, int timeToExpire)
        {
            return _storageDataAccess.SetFileReadPermission(ConfigurationManager.AppSettings.Get("azureStorageBlobLink")+blobLink, timeToExpire);
        }
    }
}
