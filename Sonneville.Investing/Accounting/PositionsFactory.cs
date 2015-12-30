using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Accounting
{
    public class PositionsFactory
    {
        private readonly HeldSharesCalculator _heldSharesCalculator;

        public PositionsFactory(HeldSharesCalculator heldSharesCalculator)
        {
            _heldSharesCalculator = heldSharesCalculator;
        }

        public IEnumerable<Position> CreateOpenPositions(IReadOnlyList<IShareTransaction> shareTransactions,
            IEnumerable<PriceQuote> priceQuotes)
        {
            var quoteDictionary = priceQuotes.ToDictionary(quote => quote.Ticker, quote => quote);
            return _heldSharesCalculator.CountHeldShares(shareTransactions)
                .Select(sharesByTicker =>
                {
                    var ticker = sharesByTicker.Key;
                    var matchingPriceQuote = quoteDictionary[ticker];
                    return new Position
                    {
                        Ticker = ticker,
                        PerSharePrice = matchingPriceQuote.PerSharePrice,
                        DateTime = matchingPriceQuote.DateTime,
                        Shares = sharesByTicker.Value
                    };
                });
        }
    }
}