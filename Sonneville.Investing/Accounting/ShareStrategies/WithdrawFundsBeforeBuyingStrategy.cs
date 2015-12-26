using System;
using Sonneville.Investing.Accounting.Cash.Transactions;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Accounting.ShareStrategies
{
    public class WithdrawFundsBeforeBuyingStrategy : IShareTransactionStrategy<Buy>
    {
        public void ProcessTransaction(IShareAccount shareAccount, Buy buy)
        {
            shareAccount.Withdraw(new Withdrawal(DateTime.Now, buy.Amount,
                $"Automatic withdrawal of {buy.Amount:C} for buying {buy.Ticker}"));
        }
    }
}