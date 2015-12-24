using System;

namespace Sonneville.Investing.Accounting.Transactions
{
    public class Deposit : IDeposit
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