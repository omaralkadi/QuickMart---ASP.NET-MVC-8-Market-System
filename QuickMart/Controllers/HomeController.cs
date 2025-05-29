using BLL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickMart.Models;
using System.Diagnostics;

namespace QuickMart.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string CategoryName)
        {

            ViewBag.Categories = await _unitOfWork.CategoryRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(CategoryName))
            {
                var FilterdProducts = await _unitOfWork.ProductRepo.GetProductsByCategoryAsync(CategoryName);
                return View(FilterdProducts);
            }
            else
            {

                var Products = await _unitOfWork.ProductRepo.GetAllAsync();
                return View(Products);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var Product=await _unitOfWork.ProductRepo.GetById(id);

            return View(Product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
