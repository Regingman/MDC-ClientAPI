using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoin.Models
{
	public class MappingRequest
	{
		/// <summary>
		/// The user's email.
		/// </summary>
		/// <example>example@example.com</example>
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		[Required(ErrorMessage = "Email cannot be null")]
		public string Email { get; set; }

		/// <summary>
		/// The social network. Only 3 options: meta, google, apple
		/// </summary>
		/// <example>meta</example>
		[Required(ErrorMessage = "SocialNetwork cannot be null")]
		public string SocialNetwork { get; set; }

		/// <summary>
		/// The user's id of specified social network
		/// </summary>
		/// <example>103954834320819139969</example>
		[Required(ErrorMessage = "SocialId cannot be null")]
		public string SocialId { get; set; }
	}
}

