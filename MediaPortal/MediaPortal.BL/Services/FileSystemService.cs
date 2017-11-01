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

namespace MediaPortal.BL.Services
{  
    public class FileSystemService : IFileSystemService
    {
        private readonly IRepository<FileSystem> _fileSyatemRepository;

        public FileSystemService(IRepository<FileSystem> fileSyatemRepository)
        {
            _fileSyatemRepository = fileSyatemRepository;
        }

        public IEnumerable<FileSystemDTO> GetAllUserFileSystem(string userId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>());

            var fileSystems = _fileSyatemRepository.GetAll();

            return Mapper.Map<IEnumerable<FileSystem>, List<FileSystemDTO>>(fileSystems);
        }

        public IEnumerable<FileSystemDTO> GetUserFileSystem(string userId, string fileSystemParentId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>());

            var fileSystem = _fileSyatemRepository.GetAll(userId, fileSystemParentId);

            return Mapper.Map<IEnumerable<FileSystem>, List<FileSystemDTO>>(fileSystem);
        }
    }
}
