using System;

namespace Sonneville.Investing.Accounting.Transactions
{
    public class Deposit : ICashTransaction
    {
        public Deposit(DateTime settlementDate, decimal amount)
        {
            SettlementDate = settlementDate;
            Amount = amount;
        }

        public DateTime SettlementDate { get; }

        public decimal Amount { get; }
    }
}