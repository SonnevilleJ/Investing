using System;
using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Accounting.Cash.Transactions;

namespace Sonneville.Investing.Accounting.Cash
{
    public class CashAccountBalanceCalculator : ICashAccountBalanceCalculator
    {
        public decimal CalculateBalance(DateTime dateTime, IEnumerable<ICashTransaction> cashTransactions)
        {
            return cashTransactions
                .Where(transaction => transaction.SettlementDate <= dateTime)
                .Sum(transaction => transaction.Amount);
        }
    }
}