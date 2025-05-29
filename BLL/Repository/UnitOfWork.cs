using BLL.Interface;
using DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuickMartContext _quickMartContext;
        public ICategoryRepo CategoryRepo { get; set; }
        public IProductRepo ProductRepo { get; set; }
        public ICartRepo CartRepo { get; set; }
        public ICartItemRepo CartItemRepo { get; set; }
        public IOrderRepo OrderRepo { get; set; }
        public IOrderItemRepo OrderItemRepo { get; set; }
        public IPaymobService PaymobService { get; set; }

        public UnitOfWork(QuickMartContext quickMartContext, ICategoryRepo categoryRepo, IProductRepo productRepo, ICartRepo cartRepo, ICartItemRepo cartItemRepo, IOrderRepo orderRepo, IOrderItemRepo orderItemRepo,IPaymobService paymobService)
        {
            _quickMartContext = quickMartContext;
            CategoryRepo = categoryRepo;
            ProductRepo = productRepo;
            CartRepo = cartRepo;
            CartItemRepo = cartItemRepo;
            OrderRepo = orderRepo;
            OrderItemRepo = orderItemRepo;
            PaymobService = paymobService;
        }
        public async Task<int> Complete()
        {
            return await _quickMartContext.SaveChangesAsync();
        }

        public ValueTask DisposeAsync()
        {
            return _quickMartContext.DisposeAsync();
        }
    }
}
