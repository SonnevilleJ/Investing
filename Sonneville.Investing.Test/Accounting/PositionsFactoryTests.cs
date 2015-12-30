using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Accounting
{
    [TestFixture]
    public class PositionsFactoryTests
    {
        [Test]
        public void ShouldCountCurrentSharesAndIgnoreClosedPositions()
        {
            var shareTransactions = new List<IShareTransaction>
            {
                new Buy(DateTime.Today, "ticker1", 1, 2, 3),
                new Buy(DateTime.Today, "ticker2", 4, 5, 6),
                new Buy(DateTime.Today, "ticker3", 4, 1, 1),
                new Sell(DateTime.Today, "ticker1", 1, 3, 3),
                new Sell(DateTime.Today, "ticker3", 2, 3, 6),
            };

            var priceQuotes = new List<PriceQuote>
            {
                new PriceQuote {Ticker = "ticker1", PerSharePrice = 20, DateTime = new DateTime(2015, 12, 30)},
                new PriceQuote {Ticker = "ticker2", PerSharePrice = 30, DateTime = new DateTime(2015, 12, 31)},
                new PriceQuote {Ticker = "ticker3", PerSharePrice = 40, DateTime = new DateTime(2015, 12, 29)},
            };

            var calculator = new PositionsFactory(new HeldSharesCalculator());
            var positions = calculator.CreateOpenPositions(shareTransactions, priceQuotes).ToList();

            Assert.AreEqual(2, positions.Count());

            var ticker2Position = positions.Single(position => position.Ticker == "ticker2");
            AssertPosition(ticker2Position, new DateTime(2015, 12, 31), 30, 4);

            var ticker3Position = positions.Single(position => position.Ticker == "ticker3");
            AssertPosition(ticker3Position, new DateTime(2015, 12, 29), 40, 2);
        }

        private static void AssertPosition(Position ticker2Position, DateTime dateTime, int perSharePrice, int shares)
        {
            Assert.AreEqual(dateTime, ticker2Position.DateTime);
            Assert.AreEqual(perSharePrice, ticker2Position.PerSharePrice);
            Assert.AreEqual(shares, ticker2Position.Shares);
            Assert.AreEqual(perSharePrice*shares, ticker2Position.Value);
        }
    }
}