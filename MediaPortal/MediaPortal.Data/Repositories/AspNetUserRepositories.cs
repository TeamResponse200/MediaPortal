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
    public class AspNetUserRepositories: IRepository<AspNetUser>
    {
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        public AspNetUserRepositories(MediaPortalDbContext connectionString)
        {
            _mediaPortalDbContext = connectionString;
        }

        public AspNetUser Get(int id)
        {
            return _mediaPortalDbContext.AspNetUsers.Find(id);
        }

        public IEnumerable<AspNetUser> GetAll()
        {
            return _mediaPortalDbContext.AspNetUsers;
        }
    }
}
