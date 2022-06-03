using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDataCoin.Models
{
    [NotMapped]
    public class AuthenticateRequest
    {
        [Required(ErrorMessage = "SocialId cannot be null")]
        public string SocialId { get; set; }

        [EmailAddress(ErrorMessage = "Not a valid email")]
        [Required(ErrorMessage = "Email cannot be null")]
        public string Email { get; set; }

        [Required(ErrorMessage = "NickName cannot be null")]
        public string NickName { get; set; }

        [Required(ErrorMessage = "Sosial Network name required")]
        public string SocialNetwork { get; set; }
    }
}
