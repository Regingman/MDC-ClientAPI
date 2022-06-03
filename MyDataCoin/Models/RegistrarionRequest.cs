using System.ComponentModel.DataAnnotations;

namespace MyDataCoin.Models
{
    public class RegistrationRequest
    {
        [EmailAddress(ErrorMessage = "Not a valid email")]
        [Required(ErrorMessage = "Email cannot be null")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password cannot be null")]
        public string Password { get; set; }
    }
}
