using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;

namespace Sonneville.Investing.PortfolioManager.Test
{
    [TestFixture]
    public class AllocationDifferencerTests
    {
        [Test]
        public void ShouldDiffPercentages()
        {
            var minuend = new Dictionary<Position, decimal>
            {
                {new Position {Ticker = "a"}, 0.25m},
                {new Position {Ticker = "b"}, 0.5m},
                {new Position {Ticker = "c"}, 0.25m},
            };
            var subtrahend = new Dictionary<Position, decimal>
            {
                {new Position {Ticker = "a"}, 0.5m},
                {new Position {Ticker = "b"}, 0.25m},
                {new Position {Ticker = "c"}, 0.25m},
            };

            var difference = new AllocationDifferencer().CalculateDifference(minuend, subtrahend);

            Assert.AreEqual(-0.25m, difference.Single(kvp => kvp.Key.Ticker == "a").Value);
            Assert.AreEqual(0.25m, difference.Single(kvp => kvp.Key.Ticker == "b").Value);
            Assert.AreEqual(0, difference.Single(kvp => kvp.Key.Ticker == "c").Value);
        }

        [Test]
        public void ShouldIncludeExtraTickersFromMinuend()
        {
            var minuend = new Dictionary<Position, decimal>
            {
                {new Position {Ticker = "a"}, 0.25m},
                {new Position {Ticker = "b"}, 0.5m},
                {new Position {Ticker = "c"}, 0.25m},
                {new Position {Ticker = "d"}, 0.25m},
            };
            var subtrahend = new Dictionary<Position, decimal>
            {
                {new Position {Ticker = "a"}, 0.5m},
                {new Position {Ticker = "b"}, 0.25m},
                {new Position {Ticker = "c"}, 0.25m},
            };

            var difference = new AllocationDifferencer().CalculateDifference(minuend, subtrahend);

            Assert.AreEqual(-0.25m, difference.Single(kvp => kvp.Key.Ticker == "a").Value);
            Assert.AreEqual(0.25m, difference.Single(kvp => kvp.Key.Ticker == "b").Value);
            Assert.AreEqual(0, difference.Single(kvp => kvp.Key.Ticker == "c").Value);
            Assert.AreEqual(0.25m, difference.Single(kvp => kvp.Key.Ticker == "d").Value);
        }

        [Test]
        public void ShouldIncludeExtraTickersFromSubtrahend()
        {
            var minuend = new Dictionary<Position, decimal>
            {
                {new Position {Ticker = "a"}, 0.25m},
                {new Position {Ticker = "b"}, 0.5m},
                {new Position {Ticker = "c"}, 0.25m},
            };
            var subtrahend = new Dictionary<Position, decimal>
            {
                {new Position {Ticker = "a"}, 0.5m},
                {new Position {Ticker = "b"}, 0.25m},
                {new Position {Ticker = "c"}, 0.25m},
                {new Position {Ticker = "d"}, 0.25m},
            };

            var difference = new AllocationDifferencer().CalculateDifference(minuend, subtrahend);

            Assert.AreEqual(-0.25m, difference.Single(kvp => kvp.Key.Ticker == "a").Value);
            Assert.AreEqual(0.25m, difference.Single(kvp => kvp.Key.Ticker == "b").Value);
            Assert.AreEqual(0, difference.Single(kvp => kvp.Key.Ticker == "c").Value);
            Assert.AreEqual(-0.25m, difference.Single(kvp => kvp.Key.Ticker == "d").Value);
        }
    }
}