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
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        public SharedAccessRepository(MediaPortalDbContext connectionString)
        {
            _mediaPortalDbContext = connectionString;
        }

        public SharedAccess Get(int id)
        {
            return _mediaPortalDbContext.SharedAccesses.Find(id);
        }

        public IEnumerable<SharedAccess> GetAll()
        {
            return _mediaPortalDbContext.SharedAccesses;
        }
    }
}
