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
    public class OrderRepo : GenericRespository<Order>, IOrderRepo
    {
        private readonly QuickMartContext _quickMartContext;
        public OrderRepo(QuickMartContext quickMartContext) : base(quickMartContext)
        {
            _quickMartContext = quickMartContext;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _quickMartContext.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();
        }
    }
}
