using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Accounting.Securities
{
    public class HeldSharesCalculator : IHeldSharesCalculator
    {
        public decimal CountHeldShares(string ticker, IEnumerable<IShareTransaction> shareTransactions)
        {
            return shareTransactions
                .Where(transaction => transaction.Ticker == ticker)
                .Sum(transaction => transaction.Shares);
        }
    }
}