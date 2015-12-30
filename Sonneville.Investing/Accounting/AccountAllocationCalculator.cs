using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Accounting
{
    public class AccountAllocationCalculator
    {
        private readonly HeldSharesCalculator _heldSharesCalculator;

        public AccountAllocationCalculator(HeldSharesCalculator heldSharesCalculator)
        {
            _heldSharesCalculator = heldSharesCalculator;
        }

        public ISecuritiesAllocation CalculateAllocation(IReadOnlyList<IShareTransaction> shareTransactions,
            IDictionary<string, decimal> priceQuotes)
        {
            var amounts = _heldSharesCalculator.ExtractTickersWithCurrentShares(shareTransactions)
                .ToDictionary(ticker => ticker,
                    ticker => _heldSharesCalculator.CountHeldShares(ticker, shareTransactions)*priceQuotes[ticker]);

            var ratesByTicker = amounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value/amounts.Sum(kvp1 => kvp1.Value));
            return SecuritiesAllocation.FromDictionary(ratesByTicker);
        }
    }
}