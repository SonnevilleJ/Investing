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
            return _heldSharesCalculator.CountHeldShares(shareTransactions)
                .Select(sharesByTicker => CreatePosition(priceQuotes, sharesByTicker.Key, sharesByTicker.Value));
        }

        private static Position CreatePosition(IEnumerable<PriceQuote> priceQuotes, string ticker, decimal shares)
        {
            var matchingPriceQuote = priceQuotes.Single(quote => quote.Ticker == ticker);
            return new Position
            {
                Ticker = ticker,
                PerSharePrice = matchingPriceQuote.PerSharePrice,
                DateTime = matchingPriceQuote.DateTime,
                Shares = shares
            };
        }
    }
}