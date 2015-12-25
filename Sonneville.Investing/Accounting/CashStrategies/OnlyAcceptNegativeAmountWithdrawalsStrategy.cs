using System;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.CashStrategies
{
    public class OnlyAcceptNegativeAmountWithdrawalsStrategy : ICashTransactionStrategy<IWithdrawal>
    {
        public void ThrowIfInvalid(IWithdrawal withdrawal, ICashAccount cashAccount)
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