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
using System.Data;
using System.Threading.Tasks;

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
            return RedirectToAction("UserFiles","Home");
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
            string viewType = string.Empty;
            if (Request.Cookies["viewType"] != null)
            {
                viewType = Request.Cookies["viewType"].Value.ToString();
            }
            else
            {
                Response.Cookies["viewType"].Value = "List";
            }

            string userId = User.Identity.GetUserId();
            List<FileSystemModels> files = null;
            IEnumerable<FileSystemDTO> fileSystemDtos;

            try
            {
                fileSystemDtos = _fileSystemService.GetUserFileSystem(userId, folderID);
            }
            catch (Exception ex)
            {
                // some logic for user
                return View("Error");
            }

            //Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>());

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>()
                .ForMember(dest => dest.Tags, opt => opt.Ignore()));

            files = Mapper.Map<IEnumerable<FileSystemDTO>, List<FileSystemModels>>(fileSystemDtos);

            var folderIDs = new List<int?>();
            var folderNames = new List<string>();

            if (folderID != null)
            {
                var tupleIdName = _fileSystemService.GetFoldersIdNamePair(folderID, userId);
                folderIDs = tupleIdName.Item1;
                folderNames = tupleIdName.Item2;
            }

            var viewModel = new UserFilesViewModels() { Files = files, FolderIDs = folderIDs, FolderNames = folderNames };

            if (viewType.Equals("BlockView"))
            {
                return View("UserFilesBlock", viewModel);
            }

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SearchFiles(string searchValue)
        {
            string viewType = string.Empty;
            if (Request.Cookies["viewType"] != null)
            {
                viewType = Request.Cookies["viewType"].Value.ToString();
            }
            else
            {
                Response.Cookies["viewType"].Value = "List";
            }

            string userId = User.Identity.GetUserId();
            List<FileSystemModels> files = null;
            IEnumerable<FileSystemDTO> allFileSystemDtos;
            IEnumerable<FileSystemDTO> fileSystemDtos;

            try
            {
                allFileSystemDtos = _fileSystemService.GetAllUserFileSystem(userId);
                fileSystemDtos = allFileSystemDtos.Where(file => file.Name.ToLower().Contains(searchValue.ToLower()));
            }
            catch (Exception)
            {
                // some logic for user
                return View("Error");
            }

            //Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>());

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemDTO, FileSystemModels>()
                .ForMember(dest => dest.Tags, opt => opt.Ignore()));

            files = Mapper.Map<IEnumerable<FileSystemDTO>, List<FileSystemModels>>(fileSystemDtos);

            var viewModel = new UserFilesViewModels() { Files = files,FolderIDs = new List<int?>(),FolderNames = new List<string>()};

            if (viewType.Equals("BlockView"))
            {
                return View("UserFilesBlock", viewModel);
            }

            return View("UserFiles", viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateFolder(FileSystemModels model, string returnUrl)
        {

            model.UserId = User.Identity.GetUserId();
            model.Type = "Folder";
            model.CreationDate = DateTime.Now;
            model.UploadDate = DateTime.Now;

            Mapper.Initialize(cfg => cfg.CreateMap<FileSystemModels, FileSystemDTO>());
            var fileSystem = Mapper.Map<FileSystemDTO>(model);

            try
            {
                _fileSystemService.InsertFileSystem(fileSystem);
            }
            catch (DataException)
            {
                // some logic for user
                return View("Error");
            }
            if (model.ParentId == null)
            {
                return RedirectToAction("UserFiles");
            }
            else
            {
                return RedirectToAction("UserFiles", new { folderID = model.ParentId, folderName = model.Name });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadFiles(FilesToUploadModels model)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<FilesToUploadModels, FilesToUploadDTO>());
            var filesToUpload = Mapper.Map<FilesToUploadDTO>(model);
            try
            {
                _fileSystemService.UploadAndInsertFiles(filesToUpload);
            }
            catch (Exception ex)
            {
                return View("Error");
            }


            return RedirectToAction("UserFiles", new { folderID = model.ParrentID });
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteFileSystem(int[] fileSystemsId)
        {
            try
            {
                _fileSystemService.DeleteFileSystem(fileSystemsId);
            }
            catch (DataException)
            {
                // some logic for user
                return View("Error");
            }

            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.T‌​oString());
        }

        [Authorize]
        [HttpPost]
        public ActionResult DownloadFileSystem(int fileSystemId, string fileSystemName)
        {
            byte[] fileBytes;

            try
            {
                fileBytes = _fileSystemService.DownloadFileSystem(fileSystemId);
            }
            catch (DataException)
            {
                // some logic for user
                return View("Error");
            }
            
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileSystemName);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DownloadFileSystemZIP(int[] fileSystemsId)
        {
            try
            {
                _fileSystemService.DownloadFileSystemZIP(fileSystemsId);
            }
            catch (DataException)
            {
                // some logic for user
                return View("Error");
            }

            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.T‌​oString());
        }

        [Authorize]
        [HttpPost]
        public ActionResult RenameFileSystem(FileSystemModels model, string returnUrl)
        {
            try
            {
                _fileSystemService.RenameFileSystem(model.Id, model.Name);
            }
            catch (DataException)
            {
                // some logic for user
                return View("Error");
            }

            return RedirectToAction("UserFiles", new { folderID = model.ParentId, folderName = model.Name });
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddTag(FileSystemModels model, string tegName)
        {
            try
            {
                _fileSystemService.AddTag(model.Id, tegName);
            }
            catch (DataException)
            {
                // some logic for user
                return View("Error");
            }

            return RedirectToAction("UserFiles", new { folderID = model.ParentId, folderName = model.Name });
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View("Error");
        }

        public async Task<ActionResult> GetImage(int fileSystemId)
        {
            var fileImageStream = await _fileSystemService.GetFileSystemThumbnailAsync(fileSystemId);

            return File(fileImageStream, "image/png");
        }
    }
}