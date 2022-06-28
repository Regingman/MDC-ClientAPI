using System;
namespace MyDataCoin.Models
{
	public class MonetizationModel
	{
		/// <summary>
		/// Total amount of views
		/// </summary>
		/// <example>201</example>
		public int Views { get; set; }

		/// <summary>
		/// Total amount of clicks
		/// </summary>
		/// <example>45</example>
		public int Clicks { get; set; }

		/// <summary>
		/// Total amount of conversions
		/// </summary>
		/// <example>34</example>
		public int Conversions { get; set; }
	}
}

