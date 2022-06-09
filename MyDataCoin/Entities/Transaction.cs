using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoin.Entities
{
    public class Transaction
    {
        /// <summary>
        /// Directions: 1 - Send, 2 - Receive
        /// Types: 1 - Ads, 2 - Refferal, 3 - Normal transaction
        /// </summary>

        [Key]
        public Guid TxId { get; set; }

        public Guid From { get; set; }

        public Guid To { get; set; }

        public double Amount { get; set; }

        public double AmountInUsd { get; set; }

        public DateTime TxDate { get; set; }

        public int Direction { get; set; }

        public int Type { get; set; }
    }
}
