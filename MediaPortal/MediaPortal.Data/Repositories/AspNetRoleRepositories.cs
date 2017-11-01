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
        private string _connectionString;

        public AspNetRoleRepositories(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AspNetRole Get(string userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.AspNetRoles.Find(userId);
            }
        }

        public IEnumerable<AspNetRole> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.AspNetRoles;
            }
        }

        public IEnumerable<AspNetRole> GetAll(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AspNetRole> GetAll(string userId, string fileSystemParentId)
        {
            throw new NotImplementedException();
        }
    }
}
