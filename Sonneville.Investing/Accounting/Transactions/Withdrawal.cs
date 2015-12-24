using System;

namespace Sonneville.Investing.Accounting.Transactions
{
    public class Withdrawal : IWithdrawal
    {
        public Withdrawal(DateTime settlementDate, decimal amount)
        {
            SettlementDate = settlementDate;
            Amount = -amount;
        }

        public DateTime SettlementDate { get; }

        public decimal Amount { get; }
    }
}