using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoin.Entities
{
    public class Transaction
    {
        //public Transaction(Guid txid, Guid from, Guid to, double amount, double amountInUsd, DateTime txDate, int direction)
        //{
        //    TxId = txid;
        //    From = from;
        //    To = to;
        //    Amount = amount;
        //    AmountInUsd = amountInUsd;
        //    TxDate = txDate;
        //    Direction = direction;
        //}

        [Key]
        public Guid TxId { get; set; }

        public Guid From { get; set; }

        public Guid To { get; set; }

        public double Amount { get; set; }

        public double AmountInUsd { get; set; }

        public DateTime TxDate { get; set; }

        public int Direction { get; set; }
    }
}
