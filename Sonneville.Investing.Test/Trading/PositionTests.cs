using System;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class PositionTests
    {
        [Test]
        [TestCase("a", false, false, 5, 25)]
        [TestCase("b", false, true, 1000000000, 1000000000)]
        [TestCase("c", true, false, -1000000000, 1000000000)]
        public void ShouldReturnValuesSpecified(string ticker, bool isCore, bool isMargin, int shares, int perSharePrice)
        {
            var position = new Position
            {
                Ticker = ticker,
                IsCore = isCore,
                IsMargin = isMargin,
                Shares = shares,
                PerSharePrice = perSharePrice,
                DateTime = DateTime.Today
            };

            Assert.AreEqual(ticker, position.Ticker);
            Assert.AreEqual(isCore, position.IsCore);
            Assert.AreEqual(isMargin, position.IsMargin);
            Assert.AreEqual(shares, position.Shares);
            Assert.AreEqual(perSharePrice, position.PerSharePrice);
            Assert.AreEqual(DateTime.Today, position.DateTime);
            Assert.AreEqual(shares*(decimal) perSharePrice, position.Value);
        }
    }
}