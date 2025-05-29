using BLL.Interface;
using BLL.Repository;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestPDF.Fluent;
using QuickMart.Helpers;

namespace QuickMart.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var Products = await _unitOfWork.ProductRepo.GetAllAsync();
            return View(Products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories=await _unitOfWork.CategoryRepo.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product,IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                if(Image != null)
                {
                    product.Image = DocumentSetting.UploadFile(Image, "Images");
                }
                await _unitOfWork.ProductRepo.Add(product);
                if(await _unitOfWork.Complete() <= 0)
                {
                    DocumentSetting.DeleteFile("Images",product.Image);

                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _unitOfWork.CategoryRepo.GetAllAsync();
            return View(product);
        }

        public async Task<IActionResult> Details(int id)
        {

            var Products = await _unitOfWork.ProductRepo.GetById(id);
            ViewBag.Categories = await _unitOfWork.CategoryRepo.GetAllAsync();
            return View(Products);
        }

        public async Task<IActionResult> Update(int id)
        {
            var product = await _unitOfWork.ProductRepo.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = await _unitOfWork.CategoryRepo.GetAllAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromRoute] int id, Product product, IFormFile image)
        {
            ModelState.Remove("Image");
            if (ModelState.IsValid)
            {
                var currentProduct = await _unitOfWork.ProductRepo.GetById(id);
                if (currentProduct == null)
                {
                    return NotFound();
                }

                currentProduct.Name = product.Name;
                currentProduct.Price = product.Price;
                currentProduct.Description = product.Description;
                currentProduct.CategoryId = product.CategoryId;

                if (image != null)
                {
                    currentProduct.Image = DocumentSetting.UploadFile(image, "Images");
                }

                _unitOfWork.ProductRepo.Update(currentProduct);
                if (await _unitOfWork.Complete() > 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                // Delete the new image if save fails
                if (image != null)
                {
                    DocumentSetting.DeleteFile("Images", currentProduct.Image);
                }
            }

            ViewBag.Categories =await _unitOfWork.CategoryRepo.GetAllAsync();
            return View(product);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var Products = await _unitOfWork.ProductRepo.GetById(id);
            ViewBag.Categories = await _unitOfWork.CategoryRepo.GetAllAsync();
            return View(Products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Product product)
        {
            _unitOfWork.ProductRepo.Delete(product);
            await _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetReport()
        {
            var products=await _unitOfWork.ProductRepo.GetAllAsync();
            var document = new ProductReport(products);
            var pdf = document.GeneratePdf();
            return File(pdf, "application/pdf", "ProductReport.pdf");
        }
    }


}
