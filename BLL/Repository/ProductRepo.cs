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
    public class ProductRepo:GenericRespository<Product>,IProductRepo
    {
        private readonly QuickMartContext _quickMartContext;
        public ProductRepo(QuickMartContext quickMartContext) : base(quickMartContext)
        {
            _quickMartContext = quickMartContext;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string CategoryName)
        {
            return await _quickMartContext.Products.Where(p => p.category.Name == CategoryName).ToListAsync();
        }
    }
}
