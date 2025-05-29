using BLL.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace QuickMart.Controllers
{
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var orders = await _unitOfWork.OrderRepo.GetOrdersByUserIdAsync(userId);

        //    return View(orders);
        //}

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _unitOfWork.OrderRepo.GetOrdersByUserIdAsync(userId);

            var totalOrders = orders.Count();

            var pagedOrders = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            return View(pagedOrders);
        }
    }
}
