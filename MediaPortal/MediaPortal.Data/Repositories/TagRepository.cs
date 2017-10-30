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
    public class TagRepository: IRepository<Tag>
    {
        private readonly MediaPortalDbContext _mediaPortalDbContext;

        public TagRepository(MediaPortalDbContext connectionString)
        {
            _mediaPortalDbContext = connectionString;
        }

        public Tag Get(int id)
        {
            return _mediaPortalDbContext.Tags.Find(id);
        }

        public IEnumerable<Tag> GetAll()
        {
            return _mediaPortalDbContext.Tags;
        }
    }
}
