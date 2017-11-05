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
    public class TagRepository : IRepository<Tag>
    {
        private string _connectionString;

        public TagRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Tag Get(string userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.Tags.Find(userId);
            }
        }

        public List<Tag> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.Tags.ToList();
            }
        }
        
    }
}
