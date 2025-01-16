using BLL.Interface;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuickMart.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var Categories=await _unitOfWork.CategoryRepo.GetAllAsync();
            return View(Categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category Category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.CategoryRepo.Add(Category);
                await _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            return View(Category);
        }

        public async Task<IActionResult> Details(int id)
        {
            var Category = await _unitOfWork.CategoryRepo.GetById(id);
            return View(Category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var Category=await _unitOfWork.CategoryRepo.GetById(id);
            return View(Category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepo.Update(category);
                await _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));

            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Category = await _unitOfWork.CategoryRepo.GetById(id);
            return View(Category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Category category)
        {
            _unitOfWork.CategoryRepo.Delete(category);
            await _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
    }
}
