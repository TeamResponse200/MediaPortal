using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortal.Data.Interface
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();

        List<T> GetAll(string userId);

        List<T> GetAll(string userId, int? fileSystemParentId);

        T Get(string id);
    }
}
