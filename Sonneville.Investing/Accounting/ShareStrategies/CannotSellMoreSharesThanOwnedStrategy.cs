using System;
using Sonneville.Investing.Accounting.Cash.Transactions;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Accounting.ShareStrategies
{
    public class CannotSellMoreSharesThanOwnedStrategy : IShareTransactionStrategy<ISell>
    {
        private readonly IHeldSharesCalculator _heldSharesCalculator;

        public CannotSellMoreSharesThanOwnedStrategy(IHeldSharesCalculator heldSharesCalculator)
        {
            _heldSharesCalculator = heldSharesCalculator;
        }

        public void ProcessTransaction(IShareAccount shareAccount, ISell sell)
        {
            if (_heldSharesCalculator.CountHeldShares(sell.Ticker, shareAccount.ShareTransactions) + sell.Shares < 0)
            {
                throw new InvalidOperationException("Insufficient shares");
            }
            shareAccount.Deposit(new Deposit(sell.SettlementDate, sell.Amount));
        }
    }
}