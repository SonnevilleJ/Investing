using System;
using Sonneville.Investing.Domain;

namespace Sonneville.Investing.Trading
{
    public class Order
    {
        public string Ticker { get; set; }

        public TransactionType TransactionType { get; set; }

        public DateTime CreationDate { get; set; }

        public decimal Shares { get; set; }

        public decimal PricePerShare { get; set; }
    }
}