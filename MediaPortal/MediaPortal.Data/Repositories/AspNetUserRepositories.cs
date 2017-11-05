﻿using MediaPortal.Data.DataAccess;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Repositories
{
    public class AspNetUserRepositories : IRepository<AspNetUser>
    {
        private string _connectionString;

        public AspNetUserRepositories(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AspNetUser Get(string userId)
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.AspNetUsers.Find(userId);
            }
        }

        public List<AspNetUser> GetAll()
        {
            using (var dbContext = new MediaPortalDbContext(_connectionString))
            {
                return dbContext.AspNetUsers.ToList();
            }
        }

        public List<AspNetUser> GetAll(string userId)
        {
            throw new NotImplementedException();
        }

        public List<AspNetUser> GetAll(string userId, int? fileSystemParentId)
        {
            throw new NotImplementedException();
        }

        public void InsertObject(AspNetUser user)
        {
            throw new NotImplementedException();
        }
    }
}
