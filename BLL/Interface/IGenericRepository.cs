using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IGenericRepository<T>
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetById(int id);
        public Task Add(T entity);
        public void Update(T entity);
        public void Delete(T entity);
    }
}
