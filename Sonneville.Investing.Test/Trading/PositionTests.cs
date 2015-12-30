using System;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class PositionTests
    {
        [Test]
        [TestCase("a", 5, 25)]
        [TestCase("b", 1000000000, 1000000000)]
        [TestCase("c", -1000000000, 1000000000)]
        public void ShouldReturnValuesSpecified(string ticker, int shares, int perSharePrice)
        {
            var position = new Position
            {
                Ticker = ticker,
                Shares = shares,
                PerSharePrice = perSharePrice,
                DateTime = DateTime.Today
            };

            Assert.AreEqual(ticker, position.Ticker);
            Assert.AreEqual(shares, position.Shares);
            Assert.AreEqual(perSharePrice, position.PerSharePrice);
            Assert.AreEqual(DateTime.Today, position.DateTime);
            Assert.AreEqual(shares*(decimal) perSharePrice, position.Value);
        }
    }
}