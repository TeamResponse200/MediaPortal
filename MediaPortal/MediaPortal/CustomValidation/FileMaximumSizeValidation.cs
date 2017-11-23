using MediaPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace MediaPortal.CustomValidation
{
    public class FileMaximumSizeValidationAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var files = value as List<HttpPostedFileWrapper>;
            if (files == null)
            {
                return false;
            }

            foreach (var file in files)
            {
                if (file == null)
                {
                    continue;
                }
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (fileExtension.Equals(".mp4") && file.ContentLength > 1024*1024*50)
                {
                    return false;
                }
                else if ((fileExtension.Equals(".png") || fileExtension.Equals(".jpg")) && file.ContentLength > 1024*1024*2)
                {
                    return false;
                }
            }
            return true;
        }
    }
}