﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.BL.Models
{
    public class FileSystemDTO
    {
        public string Id { get; set; }
        
        public string UserId { get; set; }
        
        public string ParentId { get; set; }
        
        public string Name { get; set; }

        public int? Size { get; set; }
        
        public string Type { get; set; }
        
        public string BlobLink { get; set; }
        
        public string BlobThumbnail { get; set; }
    }
}
