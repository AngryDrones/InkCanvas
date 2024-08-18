using System.ComponentModel.DataAnnotations;

namespace InkCanvas.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        public string Login { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Range(16, 100, ErrorMessage = "Yeah, I don't think so")]
        public int Age { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
