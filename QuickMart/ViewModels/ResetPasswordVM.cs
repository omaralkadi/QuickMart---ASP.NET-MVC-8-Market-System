using System.ComponentModel.DataAnnotations;

namespace QuickMart.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm Password Does not Match Password")]
        public string ConfirmPassword { get; set; }
    }
}
