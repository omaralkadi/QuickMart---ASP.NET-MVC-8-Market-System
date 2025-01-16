using DAL.Entities;

namespace QuickMart.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string Phone { get; set; }
        public string PaymentMethod { get; set; } // "Visa" or "Cash"
    }
}
