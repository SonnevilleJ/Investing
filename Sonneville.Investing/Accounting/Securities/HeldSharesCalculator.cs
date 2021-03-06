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

        public IEnumerable<string> ExtractTickersWithCurrentShares(IEnumerable<IShareTransaction> shareTransactions)
        {
            return shareTransactions
                .GroupBy(transaction => transaction.Ticker)
                .Where(grouping => grouping.Sum(transaction => transaction.Shares) != 0)
                .Select(grouping => grouping.Key)
                .Distinct();
        }

        public IDictionary<string, decimal> CountHeldShares(IEnumerable<IShareTransaction> shareTransactions)
        {
            return shareTransactions.GroupBy(transaction => transaction.Ticker)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.Sum(transaction => transaction.Shares))
                .Where(kvp => kvp.Value != 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}