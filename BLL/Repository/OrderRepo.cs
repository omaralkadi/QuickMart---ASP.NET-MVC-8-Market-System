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

        public async Task<Order> GetByPaymentOrderId(string id)
        {
            return await _quickMartContext.Orders.FirstOrDefaultAsync(o => o.PaymobOrderId == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return _quickMartContext.Orders.
                Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(d => d.OrderDate);
        }
    }
}
