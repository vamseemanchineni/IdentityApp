using API.Utility;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Name (username) must be at least {2} and maximum {1} characters.")]
        [RegularExpression(StaticDetails.UserNameRegex, ErrorMessage = "Name (username) can only contain letters and numbers.")]
        public string Name { get; set; }

        private string _email;
        [Required]
        [RegularExpression(StaticDetails.EmailRegex, ErrorMessage = "Invalid Email Address.")]
        public string Email { get => _email; set => _email = value.ToLower(); }
        [Required]
        [StringLength(15,MinimumLength = 6, ErrorMessage = "Password must be at least {2} and maximum {1} characters.")]
        public string Password { get; set; }
    }
}
