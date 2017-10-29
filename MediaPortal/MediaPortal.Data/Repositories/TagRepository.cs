using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Data.DataAccess;
using MediaPortal.Data.Interface;

namespace MediaPortal.Data.Repositories
{
    public class TagRepository: ITagRepository
    {
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        public TagRepository(string connectionString)
        {
            _mediaPortalDbContext = new MediaPortalDbContext(connectionString);

        }
    }
}
