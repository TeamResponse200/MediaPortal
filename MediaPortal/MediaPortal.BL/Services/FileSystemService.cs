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

        public void DeleteFileSystem(int[] fileSystemsId)
        {
            try
            {
                foreach(var fileSystemId in fileSystemsId)
                {
                    _fileSystemRepository.DeleteFileSystem(fileSystemId);
                }                
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
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
    }
}
