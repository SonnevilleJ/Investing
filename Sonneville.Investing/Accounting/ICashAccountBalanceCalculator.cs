using System;
using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public interface ICashAccountBalanceCalculator
    {
        decimal CalculateBalance(DateTime dateTime, IEnumerable<ICashTransaction> cashTransactions);
    }
}