using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using MediaPortal.BL;
using MediaPortal.BL.Interface;

namespace MediaPortal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //var container = new Container();
            //container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            //// Register your types, for instance:
            //container.Register<IFileSystemService, FileSystemService>(Lifestyle.Scoped);
            
            //container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            //container.Verify();

            //DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

        }
    }
}
