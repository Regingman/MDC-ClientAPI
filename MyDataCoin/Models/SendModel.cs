using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoin.Models
{
	public class SendModel
	{
		[Required(ErrorMessage = "User Id cannot be null")]
		public string AddressFrom { get; set; }

		[Required(ErrorMessage = "Address cannot be null")]
		public string AddressTo { get; set; }

		[Required(ErrorMessage = "Cannot be null")]
		public double Amount { get; set; }
	}
}

