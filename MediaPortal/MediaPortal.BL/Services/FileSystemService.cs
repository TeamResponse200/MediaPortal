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

namespace MediaPortal.BL.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly IFileSystemRepository<FileSystem> _fileSystemRepository;

        private readonly StorageDataAccess _storageDataAccess;

        public FileSystemService(IFileSystemRepository<FileSystem> fileSyatemRepository)
        {
            _fileSystemRepository = fileSyatemRepository;

            _storageDataAccess = new StorageDataAccess();
        }

        
        // add blob 
        public void InsertFile(FileSystemDTO model, byte[] file, string fileName)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystem>());
            var fileSystem = Mapper.Map<FileSystem>(model);

            _storageDataAccess.Upload(file, fileName);

            if (fileSystem != null)
            {
                _fileSystemRepository.InsertObject(fileSystem);
            }

        }

        public IEnumerable<FileSystemDTO> GetAllUserFileSystem(string userId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>()
                .ForMember(dest => dest.Tags, opt => opt.Ignore()));

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
                .ForMember(dest => dest.Tags, opt => opt.Ignore()));

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

            return Mapper.Map<IEnumerable<FileSystem>, List<FileSystemDTO>>(fileSystem);
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

        public Tuple<List<int?>, List<string>> GetFoldersIdNamePair(int? folderID,string userId)
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

                var parentID = allFiles.Where(file => file.Id == folderID).FirstOrDefault().ParentId;
                if (parentID != null)
                {
                    folderIDs.Add(parentID);
                }
                folderID = parentID;
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
                    var fileSystem = _fileSystemRepository.Get(fileSystemId);

                    if (fileSystem != null)
                    {
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

                        _fileSystemRepository.DeleteFileSystem(fileSystemId);

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

        public Tuple<byte[], string> DownloadFileSystemZIP(int[] fileSystemsId)
        {
            byte[] fileBytes = null;
            string fileName = null;

            try
            {
                DownloadFileSystemTree();
                ZipArchivingFileSystemTree();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }

            return new Tuple<byte[], string>(fileBytes, fileName);
        }

        public void DownloadFileSystemTree()
        {

        }

        public void ZipArchivingFileSystemTree()
        {

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

        public void AddTag(int fileSystemId, string tegName)
        {
            //IEnumerable<Tag> tag = null;

            //tag = _tagRepository.GetTag(tegName);

            //if (tag != null)
            //{

            //}

            //Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>());
            //Mapper.Map<IEnumerable<FileSystem>, List<FileSystemDTO>>(fileSystem);


            //_fileSystemRepository.RenameFileSystem(fileSystemId, name);

        }

        public async Task<Stream> GetFileSystemThumbnailAsync(int fileSystemId)
        {
            var fileSystem = _fileSystemRepository.Get(fileSystemId);
            var blobLink = ConfigurationManager.AppSettings.Get("azureStorageBlobLink") + fileSystem.BlobThumbnail;
            Stream fileStream;
            try
            {
                fileStream = await _storageDataAccess.GetImageThumbnail(blobLink);
                return fileStream;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.InnerException.Message);
            }
            
            return null;
            
        }

        public void UploadAndInsertFiles(FilesToUploadDTO filesToUpload)
        {
             Parallel.ForEach(filesToUpload.Files, file => {
                if (file != null) {
                     var uri =  _storageDataAccess.UploadFileInBlocksAsync(file).Result;

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


                     // TODO: queue add logic
                 }
            });
        }

        private string GetFileCuttedUri(string blobLink)
        {
            var cuttedLink = blobLink.Replace(ConfigurationManager.AppSettings.Get("azureStorageBlobLink"),"");
            return cuttedLink;
        }

        public void AddTag(string fileSystemId, string tagValue)
        {
            //IEnumerable<Tag> tag = null;

            //tag = _tagRepository.GetTag(tegName);

            //if (tag != null)
            //{

            //}

            //Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>());
            //Mapper.Map<IEnumerable<FileSystem>, List<FileSystemDTO>>(fileSystem);


            //_fileSystemRepository.RenameFileSystem(fileSystemId, name);
        }
    }
}
