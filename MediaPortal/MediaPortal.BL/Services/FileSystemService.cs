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

namespace MediaPortal.BL.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly IFileSystemRepository<FileSystem> _fileSyatemRepository;

        public FileSystemService(IFileSystemRepository<FileSystem> fileSyatemRepository)
        {
            _fileSyatemRepository = fileSyatemRepository;
        }

        public List<FileSystemDTO> GetAllUserFileSystem(string userId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>());

            var fileSystems = _fileSyatemRepository.GetAll();

            return Mapper.Map<List<FileSystem>, List<FileSystemDTO>>(fileSystems);
        }

        public List<FileSystemDTO> GetUserFileSystem(string userId, int? fileSystemParentId = null)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>());
            List<FileSystem> fileSystem = null;
            try
            {
                fileSystem = _fileSyatemRepository.GetAll(userId, fileSystemParentId);
            } 
            catch (Exception)
            {
                Trace.TraceError("Current user not found");                    
            }

            return Mapper.Map<List<FileSystem>, List<FileSystemDTO>>(fileSystem);
        }

        public void InsertFileSystem(FileSystemDTO model)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystem>());
            var fileSystem = Mapper.Map<FileSystem>(model);
            _fileSyatemRepository.InsertObject(fileSystem);
        }
    }
}
