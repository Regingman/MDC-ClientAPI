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

        /// <summary>
		/// The user's email.
		/// </summary>
		/// <example>example@example.com</example>
        [EmailAddress(ErrorMessage = "Not a valid email")]
        [Required(ErrorMessage = "Email cannot be null")]
        public string Email { get; set; }

        /// <summary>
		/// The user's nickname.
		/// </summary>
		/// <example>John Doe</example>
        [Required(ErrorMessage = "NickName cannot be null")]
        public string NickName { get; set; }

        /// <summary>
		/// The name of social network used to register/login.
		/// </summary>
		/// <example>meta</example>
        [Required(ErrorMessage = "Sosial Network name required")]
        public string SocialNetwork { get; set; }

        [Required(ErrorMessage = "Device Id cannot be null")]
        public string DeviceId { get; set; }

        [Required(ErrorMessage = "FCMToken cannot be null")]
        public string FCMToken { get; set; }
    }
}
