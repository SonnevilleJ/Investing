using System;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.Strategies
{
    public class WithdrawFundsBeforeBuyingStrategy : IBuyStrategy
    {
        public void ProcessBuy(ICashAccount cashAccount, Buy buy)
        {
            cashAccount.Withdraw(new Withdrawal(DateTime.Now, buy.Amount,
                $"Automatic withdrawal of {buy.Amount:C} for buying {buy.Ticker}"));
        }
    }
}