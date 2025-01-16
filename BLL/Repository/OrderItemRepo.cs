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
    public class OrderItemRepo : GenericRespository<OrderItem>, IOrderItemRepo
    {
        public OrderItemRepo(QuickMartContext quickMartContext) : base(quickMartContext)
        {
        }
    }
}
