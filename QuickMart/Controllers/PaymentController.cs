using BLL.Interface;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickMart.Models.Payment;

namespace QuickMart.Controllers
{
    [AllowAnonymous]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;


        public PaymentController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> PaymentCallback()
        //{
        //    var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:5001";

        //    try
        //    {
        //        // Get parameters from query string
        //        var success = bool.Parse(Request.Query["success"]);
        //        var orderId = Request.Query["order"].ToString();
        //        var transactionId = Request.Query["id"].ToString();
        //        var hmac = Request.Query["hmac"].ToString();

        //        // Create object similar to your POST callback
        //        var callbackData = new PaymobCallbackData
        //        {
        //            success = success,
        //            order = new PaymobCallbackOrder { id = int.Parse(orderId) },
        //            id = int.Parse(transactionId)
        //        };

        //        // Use the same logic as your POST method
        //        var AllOrders = await _unitOfWork.OrderRepo.GetAllAsync();
        //        var order = AllOrders.FirstOrDefault(o => o.PaymobOrderId == orderId);


        //        if (order != null)
        //        {
        //            if (success)
        //            {
        //                order.PaymentStatus = "Paid";
        //                order.PaymobTransactionId = transactionId;
        //                order.PaymobOrderId = orderId;
        //                order.PaymentDate = DateTime.Now;

        //                var cartItems = await _unitOfWork.CartRepo.GetCartItemsByUserIdAsync(order.UserId);
        //                foreach (var cartItem in cartItems)
        //                {
        //                    _unitOfWork.CartItemRepo.Delete(cartItem);
        //                }
        //                await _unitOfWork.Complete();
        //                return Redirect($"{baseUrl}/Payment/PaymentSuccess?orderId={order.Id}");
        //            }
        //            else
        //            {
        //                order.PaymentStatus = "Failed";
        //                await _unitOfWork.Complete();
        //                return Redirect($"{baseUrl}/Payment/PaymentFailed?orderId={order.Id}");
        //            }
        //        }
        //        else
        //        {
        //            return Redirect($"{baseUrl}/Payment/PaymentFailed?orderId={order.Id}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Redirect($"{baseUrl}/Payment/PaymentFailed");
        //    }
        //}

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                // Step 1: Read request body
                var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

                var receivedHmac = Request.Query["hmac"].ToString();

                //bool isValid = _unitOfWork.PaymobService.VerifyCallback(receivedHmac, requestBody);
                //if (!isValid)
                //{
                //    return Unauthorized("Invalid HMAC signature");
                //}

                var json = JObject.Parse(requestBody);
                var StringJson = json["obj"]?.ToString(Formatting.None);

                var callbackData = JsonConvert.DeserializeObject<PaymobCallbackData>(StringJson);
                if (callbackData == null || callbackData.order == null)
                {
                    return BadRequest("Invalid callback data");
                }

                var orderId = callbackData.order.id.ToString();
                var order = (await _unitOfWork.OrderRepo.GetAllAsync())
                                .FirstOrDefault(o => o.PaymobOrderId == orderId);

                if (order == null)
                {
                    return NotFound("Order not found");
                }

                if (callbackData.success == true)
                {
                    order.PaymentStatus = "Paid";
                    order.PaymobTransactionId = callbackData.id.ToString();
                    order.PaymentDate = DateTime.UtcNow;

                    var cartItems = await _unitOfWork.CartRepo.GetCartItemsByUserIdAsync(order.UserId);
                    foreach (var item in cartItems)
                    {
                        _unitOfWork.CartItemRepo.Delete(item);
                    }

                    await _unitOfWork.Complete();
                }
                else
                {
                    order.PaymentStatus = "Failed";
                    await _unitOfWork.Complete();
                }

                return Ok(new { status = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PaymentReturn(
                [FromQuery] bool success,
                [FromQuery(Name = "order")] string orderId)
        {
            if (success)
            {
                return RedirectToAction("PaymentSuccess", "Payment", new { orderId });
            }

            return RedirectToAction("PaymentFailed", "Payment", new { orderId });
        }

        public async Task<IActionResult> PaymentSuccess(string orderId)
        {
            var order = await _unitOfWork.OrderRepo.GetByPaymentOrderId(orderId);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> PaymentFailed(string orderId)
        {
            var order = await _unitOfWork.OrderRepo.GetByPaymentOrderId(orderId);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

    }
}
