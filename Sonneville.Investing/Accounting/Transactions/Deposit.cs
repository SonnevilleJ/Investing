using System;

namespace Sonneville.Investing.Accounting.Transactions
{
    public class Deposit : IDeposit
    {
        public Deposit(DateTime settlementDate, decimal amount, string memo = null)
        {
            SettlementDate = settlementDate;
            Amount = amount;
            Memo = memo ?? string.Empty;
        }

        public DateTime SettlementDate { get; }

        public decimal Amount { get; }

        public string Memo { get; }
    }
}