using System.ComponentModel.DataAnnotations;

namespace QuickMart.ViewModels
{
    public class RegisterVM
    {
        public string FName { get; set; }
        public string LName { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Confirm Password Does not match Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        public bool Agree { get; set; }
    }
}
