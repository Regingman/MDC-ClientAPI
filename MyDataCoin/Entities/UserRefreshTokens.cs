using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoin.Entities
{
	public class UserRefreshTokens
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string SocialId { get; set; }

		[Required]
		public string RefreshToken { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
