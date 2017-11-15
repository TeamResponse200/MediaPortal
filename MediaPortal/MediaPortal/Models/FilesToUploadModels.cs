using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaPortal.Models
{
    public class FilesToUploadModels
    {
        public string UserID { get; set; }
        public int? ParrentID { get; set; }
        public IList<HttpPostedFileBase> Files { get; set; }
    }
}