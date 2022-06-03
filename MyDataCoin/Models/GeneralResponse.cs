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

        public int Code { get; set; }

        public string Message { get; set; }
        
    }
}
