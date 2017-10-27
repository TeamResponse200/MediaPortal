using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.BL.Models
{
    class FileTagDTO
    {
        public int Id { get; set; }

        public int FileSystemId { get; set; }

        public int TagId { get; set; }
    }
}
