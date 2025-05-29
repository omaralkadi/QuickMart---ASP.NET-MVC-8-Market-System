using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign Key to the User
        [ForeignKey("UserId")]
        public ApplicationUser applicationUser { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string Phone { get; set; }
        public string PaymentMethod { get; set; } // "Visa" or "Cash"


        //for payment
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed
        public string? PaymobOrderId { get; set; }
        public string? PaymentToken { get; set; }
        public string? PaymobTransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
