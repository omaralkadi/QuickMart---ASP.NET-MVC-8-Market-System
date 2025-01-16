using BLL.Interface;
using DAL.Context;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repository
{
    public class CategoryRepo : GenericRespository<Category>, ICategoryRepo
    {
        private readonly QuickMartContext _quickMartContext;
        public CategoryRepo(QuickMartContext quickMartContext) : base(quickMartContext)
        {
            _quickMartContext = quickMartContext;
        }


    }
}
