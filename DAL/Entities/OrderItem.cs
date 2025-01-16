using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Foreign Key to Order
        public int ProductId { get; set; } // Foreign Key to Product
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price of the product at the time of the order

        // Navigation Properties
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

    }
}
