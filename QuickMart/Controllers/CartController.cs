using BLL.Interface;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuickMart.Models.Payment;
using QuickMart.ViewModels;
using System.Security.Claims;

namespace QuickMart.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public CartController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _unitOfWork.CartRepo.GetAllAsync();

            var userCart = cart.FirstOrDefault(c => c.UserId == userId);
            if (userCart == null)
            {
                userCart = new Cart
                {
                    UserId = userId
                };

                await _unitOfWork.CartRepo.Add(userCart);
                await _unitOfWork.Complete();
            }

            var cartItems = await _unitOfWork.CartItemRepo.GetAllAsync();

            var userCartItems = cartItems.Where(c => c.CartId == userCart.Id);

            return View(userCartItems);
        }
        public async Task<IActionResult> AddToCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _unitOfWork.CartRepo.GetAllAsync();
            var userCart = cart.FirstOrDefault(c => c.UserId == userId);

            if (userCart == null)
            {
                userCart = new Cart { UserId = userId };
                await _unitOfWork.CartRepo.Add(userCart);
                await _unitOfWork.Complete();
            }

            var existingItem = (await _unitOfWork.CartItemRepo.GetAllAsync())
                .FirstOrDefault(ci => ci.CartId == userCart.Id && ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
                _unitOfWork.CartItemRepo.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = userCart.Id,
                    ProductId = productId,
                    Quantity = 1,
                    Price = (await _unitOfWork.ProductRepo.GetById(productId)).Price
                };

                await _unitOfWork.CartItemRepo.Add(cartItem);
            }

            await _unitOfWork.Complete();
            return Json(new { success = true, message = "Product added to cart!" });
        }

        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _unitOfWork.CartItemRepo.GetById(cartItemId);

            if (cartItem != null)
            {
                _unitOfWork.CartItemRepo.Delete(cartItem);
                await _unitOfWork.Complete();
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> InCreaseQuntity(int cartItemId)
        {
            var cartItem = await _unitOfWork.CartItemRepo.GetById(cartItemId);
            cartItem.Quantity++;
            _unitOfWork.CartItemRepo.Update(cartItem);
            await _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeCreaseQuntity(int cartItemId)
        {
            var cartItem = await _unitOfWork.CartItemRepo.GetById(cartItemId);
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                _unitOfWork.CartItemRepo.Update(cartItem);
                await _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _unitOfWork.CartItemRepo.Delete(cartItem);
                await _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _unitOfWork.CartRepo.GetCartItemsByUserIdAsync(userId);

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price),
                BillingAddress = "",
                ShippingAddress = ""
            };

            return View(checkoutViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _unitOfWork.CartRepo.GetCartItemsByUserIdAsync(userId);

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            // Calculate the total price
            var totalPrice = cartItems.Sum(c => c.Quantity * c.Product.Price);

            // Create the order object (order details can be adjusted as needed)
            var order = new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,
                OrderDate = DateTime.Now,
                BillingAddress = model.BillingAddress,
                ShippingAddress = model.ShippingAddress,
                Phone = model.Phone,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = "Pending",

            };

            await _unitOfWork.OrderRepo.Add(order);

            await _unitOfWork.Complete();

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.Price
                };
                await _unitOfWork.OrderItemRepo.Add(orderItem);
            }
            await _unitOfWork.Complete();

            // Handle different payment methods
            if (model.PaymentMethod == "Cash")
            {
                await _unitOfWork.Complete();

                foreach (var cartItem in cartItems)
                {
                    _unitOfWork.CartItemRepo.Delete(cartItem);
                }
                await _unitOfWork.Complete();

                return Json(new { success = true, message = "Order Placed Successfully (Cash on Delivery)" });
            }
            else if (order.PaymentMethod == "Visa")
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    var paymentResult = await _unitOfWork.PaymobService.CreatePaymentTokenAsync(order, user);

                    // Store payment token in order
                    order.PaymentToken = paymentResult.PaymentToken;
                    order.PaymobOrderId=paymentResult.PaymobOrderId;

                    await _unitOfWork.Complete();

                    return Json(new
                    {
                        success = true,
                        requiresPayment = true,
                        paymentToken = paymentResult.PaymentToken,
                        orderId = order.Id,
                        message = "Redirecting to payment..."
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Payment initialization failed: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Invalid payment method" });

        }

    }
}



