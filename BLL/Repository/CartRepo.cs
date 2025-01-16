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
    public class CartRepo : GenericRespository<Cart>, ICartRepo
    {
        private readonly QuickMartContext _quickMartContext;
        public CartRepo(QuickMartContext quickMartContext) : base(quickMartContext)
        {
            _quickMartContext = quickMartContext;
        }

        public async Task<List<CartItem>> GetCartItemsByUserIdAsync(string userId)
        {
            var CartId=await _quickMartContext.Carts.Where(c => c.UserId == userId).Select(c => c.Id).FirstOrDefaultAsync();
            return await _quickMartContext.CartItems.Where(ci => ci.CartId == CartId).Include(ci => ci.Product).ToListAsync();
        }
    }
}
