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

namespace MediaPortal.BL.Services
{  
    public class FileSystemService : IFileSystemService
    {
        public IUnitOfWork Database { get; set; }

        public FileSystemService(IUnitOfWork unitOfWork)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            Database = unitOfWork;
        }

        public void GetFileSystemByName(FileSystemDTO fileSystemDTO)
        {
            //do......
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
