﻿using System;
using System.Web.Mvc;
using Ninject;
using System.Collections.Generic;
using MediaPortal.BL.Interface;
using MediaPortal.BL.Services;

namespace MediaPortal.Util
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            kernel.Bind<IFileSystemService>().To<FileSystemService>();
        }
    }
}