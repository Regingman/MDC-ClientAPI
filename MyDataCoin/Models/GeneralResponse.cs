using System;
namespace MyDataCoin.Models
{
    public class GeneralResponse
    {
        public GeneralResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
		/// The HTTP Status Code
		/// </summary>
		/// <example>201</example>
        public int Code { get; set; }

        /// <summary>
		/// The Message from response
		/// </summary>
		/// <example>Success</example>
        public string Message { get; set; }
        
    }
}
