using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Data.DataAccess;
using MediaPortal.Data.Interface;

namespace MediaPortal.Data.Repositories
{
    public class SharedAccessRepository : ISharedAccessRepository
    {
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        public SharedAccessRepository(string connectionString)
        {
            _mediaPortalDbContext = new MediaPortalDbContext(connectionString);

        }
    }
}
