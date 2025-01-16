using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IUnitOfWork:IAsyncDisposable
    {
        public ICategoryRepo CategoryRepo { get; }
        public IProductRepo ProductRepo { get; }
        public ICartRepo CartRepo { get; }
        public ICartItemRepo CartItemRepo { get; }
        public IOrderRepo OrderRepo { get; }
        public IOrderItemRepo OrderItemRepo { get; }
        public Task<int> Complete();
    }
}
