using System;

namespace Sonneville.Investing.Accounting.Transactions
{
    public class Sell : IShareTransaction
    {
        public Sell(DateTime settlementDate, string ticker, decimal shares, decimal price, decimal commission, string memo = null)
        {
            SettlementDate = settlementDate;
            Ticker = ticker;
            Shares = -shares;
            PerSharePrice = price;
            Commission = commission;
            Memo = memo ?? string.Empty;
        }

        public DateTime SettlementDate { get; }

        public decimal Amount => Commission + Shares*PerSharePrice;

        public string Memo { get; }

        public string Ticker { get; }

        public decimal Shares { get; }

        public decimal PerSharePrice { get; }

        public decimal Commission { get; }
    }
}