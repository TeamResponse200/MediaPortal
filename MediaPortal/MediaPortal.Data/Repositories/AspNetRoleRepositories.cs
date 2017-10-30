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
    public class AspNetRoleRepositories : IRepository<AspNetRole>
    {
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        public AspNetRoleRepositories(string connectionString)
        {
            _mediaPortalDbContext = new MediaPortalDbContext(connectionString);

        }

        public AspNetRole Get(int id)
        {
            return _mediaPortalDbContext.AspNetRoles.Find(id);
        }

        public IEnumerable<AspNetRole> GetAll()
        {
            return _mediaPortalDbContext.AspNetRoles;
        }
    }
}
