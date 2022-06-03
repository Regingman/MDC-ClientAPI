using System;
namespace MyDataCoin.Entities
{
    public class Transaction
    {
        public Transaction(string txid, string from, string to, double amount, double amountInUsd, DateTime txDate, int direction)
        {
            TxId = txid;
            From = from;
            To = to;
            Amount = amount;
            AmountInUsd = amountInUsd;
            TxDate = txDate;
            Direction = direction;
        }

        public string TxId { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public double Amount { get; set; }

        public double AmountInUsd { get; set; }

        public DateTime TxDate { get; set; }

        public int Direction { get; set; }
    }
}
