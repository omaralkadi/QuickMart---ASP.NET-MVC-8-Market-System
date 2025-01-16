using System.ComponentModel.DataAnnotations;

namespace QuickMart.ViewModels
{
    public class ForgetPasswordVM
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}
