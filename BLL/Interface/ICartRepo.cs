using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface ICartRepo:IGenericRepository<Cart>
    {
        Task<List<CartItem>> GetCartItemsByUserIdAsync(string userId);
    }
}
