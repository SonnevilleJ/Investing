using System;
using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.Validation
{
    public class OnlyAcceptNegativeAmountWithdrawalsValidator : ICashTransactionValidator<IWithdrawal>
    {
        public void ThrowIfInvalid(IWithdrawal withdrawal, IEnumerable<ICashTransaction> currentTransactions)
        {
            if (withdrawal == null)
            {
                throw new ArgumentNullException(nameof(withdrawal), "Cannot validate a null Withdrawal!");
            }
            if (!(withdrawal.Amount < 0))
            {
                throw new InvalidOperationException("Cannot withdraw a positive amount of funds!");
            }
        }
    }
}