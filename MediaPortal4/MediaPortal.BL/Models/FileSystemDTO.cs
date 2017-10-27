﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.BL.Models
{
    class FileSystemDTO
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }

        public int? ParentId { get; set; }
        
        public string Name { get; set; }

        public int? Size { get; set; }
        
        public string Type { get; set; }
        
        public string BlobLink { get; set; }
        
        public string BlobThumbnail { get; set; }
    }
}
