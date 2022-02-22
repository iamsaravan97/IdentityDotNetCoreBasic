using System.ComponentModel.DataAnnotations;

namespace IdentityBasicDotNetCore.Models
{
    public class SignInViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage ="Invalid Email or Email is missing")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Invalid Password or Password is missing")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public bool isTwoFactorEnabled { get; set; }

        public string Code { get; set; }

    }
}
