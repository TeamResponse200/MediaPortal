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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult File()
        {
            string userId = "";
            string parentId = "";

            IEnumerable<FileSystemDTO> fileSystemDtos = _fileSystemService.GetUserFileSystem(userId, parentId);

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>());

            var files = Mapper.Map<IEnumerable<FileSystemDTO>, List<FileSystemModels>>(fileSystemDtos);

            return View();
        }
    }
}