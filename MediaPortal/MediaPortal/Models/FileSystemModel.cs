using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaPortal.Models
{
    public class FileSystemModel
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