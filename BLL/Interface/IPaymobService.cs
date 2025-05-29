using DAL.Entities;
using DAL.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IPaymobService
    {
        Task<PaymentResult> CreatePaymentTokenAsync(Order order, ApplicationUser user);
        //bool VerifyCallback(string hmacHeader, string requestBody);
    }

}
