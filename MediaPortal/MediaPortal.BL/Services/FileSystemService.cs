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

namespace MediaPortal.BL.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly IFileSystemRepository<FileSystem> _fileSystemRepository;

        public FileSystemService(IFileSystemRepository<FileSystem> fileSyatemRepository)
        {
            _fileSystemRepository = fileSyatemRepository;
        }

        public IEnumerable<FileSystemDTO> GetAllUserFileSystem(string userId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>());

            var fileSystems = _fileSystemRepository.GetAll();

            return Mapper.Map<IEnumerable<FileSystem>, List<FileSystemDTO>>(fileSystems);
        }

        public IEnumerable<FileSystemDTO> GetUserFileSystem(string userId, int? fileSystemParentId = null)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FileSystem, FileSystemDTO>());
            IEnumerable<FileSystem> fileSystem = null;
            try
            {
                fileSystem = _fileSystemRepository.GetAll(userId, fileSystemParentId);
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
    }
}
