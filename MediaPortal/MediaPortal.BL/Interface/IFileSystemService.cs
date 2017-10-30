using MediaPortal.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.BL.Interface
{
    public interface IFileSystemService
    {
        void GetFileSystemByName(FileSystemDTO fileSystemDTO);
    }
}
