using System;
using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.CashStrategies
{
    public class OnlyAcceptPositiveOrZeroAmountDepositsStrategy : ICashTransactionStrategy<IDeposit>
    {
        public void ThrowIfInvalid(IDeposit deposit, IEnumerable<ICashTransaction> currentTransactions)
        {
            if (deposit == null)
            {
                throw new ArgumentNullException(nameof(deposit), "Cannot validate null Deposit!");
            }
            if (deposit.Amount < 0)
            {
                throw new InvalidOperationException("Cannot deposit a negative amount of funds!");
            }
        }
    }
}