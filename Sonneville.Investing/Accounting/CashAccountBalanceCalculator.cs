using System;
using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public class CashAccountBalanceCalculator
    {
        public decimal CalculateBalance(DateTime dateTime, IEnumerable<ICashTransaction> cashTransactions)
        {
            return cashTransactions
                .Where(transaction => transaction.SettlementDate <= dateTime)
                .Sum(transaction => transaction.Amount);
        }
    }
}