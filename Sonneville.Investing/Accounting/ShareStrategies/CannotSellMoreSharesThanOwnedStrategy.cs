using System;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.ShareStrategies
{
    public class CannotSellMoreSharesThanOwnedStrategy : IShareTransactionStrategy<ISell>
    {
        public void ProcessTransaction(IShareAccount shareAccount, ISell sell)
        {
            if (shareAccount.CountHeldShares(sell.Ticker) + sell.Shares < 0)
            {
                throw new InvalidOperationException("Insufficient shares");
            }
            shareAccount.Deposit(new Deposit(sell.SettlementDate, sell.Amount));
        }
    }
}