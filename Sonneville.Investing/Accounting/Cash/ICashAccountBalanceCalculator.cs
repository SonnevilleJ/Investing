using System;
using System.Collections.Generic;
using Sonneville.Investing.Accounting.Cash.Transactions;

namespace Sonneville.Investing.Accounting.Cash
{
    public interface ICashAccountBalanceCalculator
    {
        decimal CalculateBalance(DateTime dateTime, IEnumerable<ICashTransaction> cashTransactions);
    }
}