using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
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
    }
}