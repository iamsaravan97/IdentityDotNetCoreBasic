using System.ComponentModel.DataAnnotations;

namespace IdentityBasicDotNetCore.Models
{
    public class SignUpViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage ="Invalid Email or Email is missing")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Invalid Password or Password is missing")]
        public string Password { get; set; }

        [Required]
        public string Roles { get; set; }

        [Required]
        public string Department { get; set; }

    }
}
