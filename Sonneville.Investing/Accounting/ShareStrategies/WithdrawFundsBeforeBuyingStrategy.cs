using System;
using Sonneville.Investing.Accounting.Cash.Transactions;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Accounting.ShareStrategies
{
    public class WithdrawFundsBeforeBuyingStrategy : IShareTransactionStrategy<IBuy>
    {
        public void ProcessTransaction(IShareAccount shareAccount, IBuy buy)
        {
            shareAccount.Withdraw(new Withdrawal(DateTime.Now, buy.Amount,
                $"Automatic withdrawal of {buy.Amount:C} for buying {buy.Ticker}"));
        }
    }
}