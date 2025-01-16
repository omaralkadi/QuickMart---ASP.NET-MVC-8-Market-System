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
    public class CartItemRepo: GenericRespository<CartItem>, ICartItemRepo
    {
        public CartItemRepo(QuickMartContext quickMartContext) : base(quickMartContext)
        { 
        }
    }
}
