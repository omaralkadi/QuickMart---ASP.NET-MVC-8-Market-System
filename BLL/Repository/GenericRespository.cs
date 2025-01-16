using BLL.Interface;
using DAL.Context;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repository
{
    public class GenericRespository<T> : IGenericRepository<T> where T : class
    {
        private readonly QuickMartContext _quickMartContext;
        public GenericRespository(QuickMartContext quickMartContext)
        {
            _quickMartContext = quickMartContext;
        }
        public async Task Add(T entity)
        {
            await _quickMartContext.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _quickMartContext.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                return (IEnumerable<T>)await _quickMartContext.Set<Product>().Include(p=>p.category).ToListAsync();

            }
            if (typeof(T) == typeof(CartItem))
            {
                return (IEnumerable<T>)await _quickMartContext.Set<CartItem>().Include(p => p.Product).ToListAsync();

            }

            return await _quickMartContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _quickMartContext.Set<T>().FindAsync(id);
        }

        public void Update(T entity)
        {
            _quickMartContext.Update(entity);
        }
    }
}
