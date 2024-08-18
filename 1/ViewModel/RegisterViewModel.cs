using InkCanvas.ViewModel.CustomValidaitonAttributes;
using System.ComponentModel.DataAnnotations;

namespace InkCanvas.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Login is required")]
        [MinLength(6, ErrorMessage = "Login must be at least 6 characters long")]
        [MaxLength(18, ErrorMessage = "Login cannot exceed 18 characters")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"\b\w+@[\w.]+\.[A-z.]{2,4}\b", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required]
        [AgeRange(16, 100, ErrorMessage = "Enter your age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Create a password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [MaxLength(18, ErrorMessage = "Password cannot exceed 24 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Re-enter the password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
