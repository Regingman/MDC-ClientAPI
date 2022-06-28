using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoin.Models
{
	public class SendModel
	{
		/// <summary>
		/// Address from
		/// </summary>
		/// <example>mdc7708vyo8CRu9LiAv6</example>
		[Required(ErrorMessage = "User Id cannot be null")]
		public string AddressFrom { get; set; }

		/// <summary>
		/// Address to
		/// </summary>
		/// <example>mdc7708vyo8CRu9LiAv6</example>
		[Required(ErrorMessage = "Address cannot be null")]
		public string AddressTo { get; set; }

		/// <summary>
		/// Amount of coins need to be sent
		/// </summary>
		/// <example>15</example>
		[Required(ErrorMessage = "Cannot be null")]
		public double Amount { get; set; }
	}
}

