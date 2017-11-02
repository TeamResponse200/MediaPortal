using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Data.DataAccess;
using MediaPortal.Data.Interface;
using MediaPortal.Data.EntitiesModel;

namespace MediaPortal.Data.Repositories
{
    public class SharedAccessRepository : IRepository<SharedAccess>
    {
        private string _connectionString;

        public SharedAccessRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SharedAccess Get(string userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.SharedAccesses.Find(userId);
            }
        }

        public List<SharedAccess> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.SharedAccesses.ToList();
            }
        }

        public List<SharedAccess> GetAll(string userId)
        {
            throw new NotImplementedException();
        }

        public List<SharedAccess> GetAll(string userId, int? fileSystemParentId)
        {
            throw new NotImplementedException();
        }
    }
}
