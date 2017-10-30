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

namespace MediaPortal.BL.Services
{  
    public class FileSystemService : IFileSystemService
    {
        private readonly IRepository<FileSystem> _fileSyatemRepository;
        
        public FileSystemService()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            _fileSyatemRepository = new FileSystemRepository(connectionString);
        }

        public void GetFileSystemByName(FileSystemDTO fileSystemDTO)
        {
            //do......
        }        
    }
}
