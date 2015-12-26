using System;

namespace Sonneville.Investing.Accounting.Securities.Transactions
{
    public class Buy : IBuy
    {
        public Buy(DateTime settlementDate, string ticker, decimal shares, decimal perSharePrice, decimal commission, string memo = null)
        {
            SettlementDate = settlementDate;
            Ticker = ticker;
            Shares = shares;
            PerSharePrice = perSharePrice;
            Commission = commission;
            Memo = memo ?? string.Empty;
        }

        public DateTime SettlementDate { get; }

        public decimal Amount => Shares*PerSharePrice + Commission;

        public string Memo { get; }

        public string Ticker { get; }

        public decimal Shares { get; }

        public decimal PerSharePrice { get; }

        public decimal Commission { get; }
    }
}