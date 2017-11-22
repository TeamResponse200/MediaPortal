using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MediaPortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Download",
                url: "Home/DownloadFileSystem/{fileSystemId}/{fileSystemName}",
                defaults: new { controller = "Home", action = "DownloadFileSystem", fileSystemId = UrlParameter.Optional, fileSystemName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DownloadProcess",
                url: "Home/DownloadProcess/{fileSystemName}",
                defaults: new { controller = "Home", action = "DownloadProcess", fileSystemName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{folderID}/{folderName}",
                defaults: new { controller = "Home", action = "UserFiles", folderID = UrlParameter.Optional,folderName = UrlParameter.Optional }
            );
        }
    }
}
