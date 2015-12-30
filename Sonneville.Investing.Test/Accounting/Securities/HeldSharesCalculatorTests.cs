using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Test.Accounting.Securities
{
    [TestFixture]
    public class HeldSharesCalculatorTests
    {
        private HeldSharesCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new HeldSharesCalculator();
        }

        [Test]
        public void ShouldAddSharesFromBuys()
        {
            var shareTransactions = new List<IShareTransaction>
            {
                new Buy(DateTime.Today, "ticker", 1, 2, 3),
                new Buy(DateTime.Today, "ticker", 4, 5, 6),
            };

            var heldShares = _calculator.CountHeldShares("ticker", shareTransactions);

            Assert.AreEqual(5, heldShares);
        }

        [Test]
        public void ShouldSubtractSharesFromSells()
        {
            var shareTransactions = new List<IShareTransaction>
            {
                new Buy(DateTime.Today, "ticker", 1, 2, 3),
                new Buy(DateTime.Today, "ticker", 4, 5, 6),
                new Sell(DateTime.Today, "ticker", 2, 3, 4),
            };

            var heldShares = _calculator.CountHeldShares("ticker", shareTransactions);

            Assert.AreEqual(3, heldShares);
        }

        [Test]
        public void ShouldListUniqueTickers()
        {
            var shareTransactions = new List<IShareTransaction>
            {
                new Buy(DateTime.Today, "ticker1", 1, 2, 3),
                new Buy(DateTime.Today, "ticker2", 4, 5, 6),
                new Buy(DateTime.Today, "ticker3", 4, 1, 1),
                new Sell(DateTime.Today, "ticker1", 1, 3, 3),
                new Sell(DateTime.Today, "ticker3", 2, 3, 6),
            };

            var tickers = _calculator.ExtractTickersWithCurrentShares(shareTransactions).ToList();

            CollectionAssert.DoesNotContain(tickers, "ticker1");
            CollectionAssert.Contains(tickers, "ticker2");
            CollectionAssert.Contains(tickers, "ticker3");
        }

        [Test]
        public void ShouldReturnSharesByTicker()
        {
            var shareTransactions = new List<IShareTransaction>
            {
                new Buy(DateTime.Today, "ticker1", 1, 2, 3),
                new Buy(DateTime.Today, "ticker2", 4, 5, 6),
                new Buy(DateTime.Today, "ticker3", 4, 1, 1),
                new Sell(DateTime.Today, "ticker1", 1, 3, 3),
                new Sell(DateTime.Today, "ticker3", 2, 3, 6),
            };

            var sharesByTicker = _calculator.CountHeldShares(shareTransactions);

            Assert.AreEqual(4, sharesByTicker["ticker2"]);
            Assert.AreEqual(2, sharesByTicker["ticker3"]);
            Assert.AreEqual(2, sharesByTicker.Count());
        }
    }
}