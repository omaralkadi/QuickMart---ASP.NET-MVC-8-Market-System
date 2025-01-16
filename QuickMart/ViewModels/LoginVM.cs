using System.ComponentModel.DataAnnotations;

namespace QuickMart.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email Is Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
