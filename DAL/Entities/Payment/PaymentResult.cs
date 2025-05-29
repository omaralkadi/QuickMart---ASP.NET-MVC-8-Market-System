using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities.Payment
{
    public class PaymentResult
    {
        public string? PaymentToken { get; set; }
        public string? PaymobOrderId { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
