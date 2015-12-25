using System;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.ShareStrategies
{
    public class CannotSellMoreSharesThanOwnedStrategy : IShareTransactionStrategy<Sell>
    {
        public void ProcessTransaction(IShareAccount shareAccount, Sell sell)
        {
            if (shareAccount.CountHeldShares(sell.Ticker) + sell.Shares < 0)
            {
                throw new InvalidOperationException("Insufficient shares");
            }
        }
    }
}