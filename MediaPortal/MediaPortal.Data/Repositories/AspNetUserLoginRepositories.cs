using MediaPortal.Data.DataAccess;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Repositories
{
    public class AspNetUserLoginRepositories : IRepository<AspNetUserLogin>
    {
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        public AspNetUserLoginRepositories(MediaPortalDbContext connectionString)
        {
            _mediaPortalDbContext = connectionString;
        }

        public AspNetUserLogin Get(int id)
        {
            return _mediaPortalDbContext.AspNetUserLogins.Find(id);
        }

        public IEnumerable<AspNetUserLogin> GetAll()
        {
            return _mediaPortalDbContext.AspNetUserLogins;
        }
    }
}
