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
        public ActionResult GetUserFile(string userId)
        {
            userId = "e389e0d8-cbc4-4c66-8b29-60371862cdc0";            

            List<FileSystemDTO> fileSystemDtos = _fileSystemService.GetUserFileSystem(userId);

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>());

            var files = Mapper.Map<List<FileSystemDTO>, List<FileSystemModels>>(fileSystemDtos);

            return View();
        }
    }
}