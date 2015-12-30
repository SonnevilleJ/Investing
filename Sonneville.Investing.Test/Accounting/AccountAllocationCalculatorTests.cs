using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Test.Accounting
{
    [TestFixture]
    public class AccountAllocationCalculatorTests
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

            var priceQuotes = new Dictionary<string, decimal>
            {
                {"ticker1", 20 },
                {"ticker2", 30 },
                {"ticker3", 40 },
            };

            var calculator = new AccountAllocationCalculator(new HeldSharesCalculator());
            var allocation = calculator.CalculateAllocation(shareTransactions, priceQuotes);

            Assert.AreEqual(2, allocation.Count());
            Assert.AreEqual(0.6, allocation.GetAmount("ticker2"));
            Assert.AreEqual(0.4, allocation.GetAmount("ticker3"));
        }
    }
}