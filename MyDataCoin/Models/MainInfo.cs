using System;
namespace MyDataCoin.Models
{
    public class MainInfo
    {
        public MainInfo(double balance, bool unreadMessages)
        {
            Balance = balance;
            UnreadMessages = unreadMessages;
        }

        public double Balance { get; set; }

        public bool UnreadMessages { get; set; }
    }
}
