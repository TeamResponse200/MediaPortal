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

namespace MediaPortal.BL
{  
    public class FileSystemService : IFileSystemService
    {
        private readonly IFileSystemRepository _fileSyatemRepository;

        private readonly IFileTagRepository _fileTagRepository;

        private readonly ISharedAccessRepository _sharedAccessRepository;

        private readonly ITagRepository _tagRepository;

        public FileSystemService()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            _fileSyatemRepository = new FileSyatemRepository(connectionString);
            _fileTagRepository = new FileTagRepository(connectionString);
            _sharedAccessRepository = new SharedAccessRepository(connectionString);
            _tagRepository = new TagRepository(connectionString);
        }

        public void DoSth()
        {
            //do......
        }
    }
}
