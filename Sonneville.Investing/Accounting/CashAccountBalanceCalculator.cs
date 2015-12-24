using System;
using System.Linq;

namespace Sonneville.Investing.Accounting
{
    public class CashAccountBalanceCalculator
    {
        public decimal CalculateBalance(DateTime dateTime, ICashAccount cashAccount)
        {
            return cashAccount.CashTransactions
                .Where(transaction => transaction.SettlementDate <= dateTime)
                .Sum(transaction => transaction.Amount);
        }
    }
}