using System.ComponentModel.DataAnnotations;

namespace MVCFaculty.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
