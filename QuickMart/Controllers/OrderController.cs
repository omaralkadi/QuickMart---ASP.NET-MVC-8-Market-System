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

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _unitOfWork.OrderRepo.GetOrdersByUserIdAsync(userId);

            return View(orders);
        }
    }
}
