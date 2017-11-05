using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MediaPortal.BL;
using MediaPortal.BL.Interface;
using Serilog;
using MediaPortal.BL.Models;
using AutoMapper;
using MediaPortal.Models;
using Microsoft.AspNet.Identity;

namespace MediaPortal.Controllers
{
    public class HomeController : Controller
    {
        private IFileSystemService _fileSystemService;

        public HomeController(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult UserFiles(int? folderID, string folderName)
        {
            string userId = User.Identity.GetUserId();
            List<FileSystemModels> files = null;
            List<FileSystemDTO> fileSystemDtos;

            try
            {
                fileSystemDtos = _fileSystemService.GetUserFileSystem(userId, folderID);
            }
            catch (Exception)
            {
                return View("Error");
            }

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>());
            files = Mapper.Map<List<FileSystemDTO>, List<FileSystemModels>>(fileSystemDtos);

            var folders = files.Where(m => m.Type.Equals("Folder")).ToList();
            ViewBag.Folders = folders;

            if (folderID != null)
            {
                ViewBag.FolderPath = folderName;
            }

            return View(files);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateFolder(FileSystemModels model,string returnUrl)
        {
            model.UserId = User.Identity.GetUserId();
            model.Type = "Folder";

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemModels,FileSystemDTO>());
            var fileSystem = Mapper.Map<FileSystemDTO>(model);

            _fileSystemService.InsertFileSystem(fileSystem);

            return RedirectToAction("UserFiles", "Home");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View("Error");
        }
    }
}