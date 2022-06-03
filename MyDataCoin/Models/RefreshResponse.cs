using System;
using MyDataCoin.Entities;

namespace MyDataCoin.Models
{
    public class RefreshResponse
    {
        public RefreshResponse(int code, Tokens tokens)
        {
            Code = code;
            Tokens = tokens;
        }

        public RefreshResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public Tokens Tokens { get; set; }
    }
}
