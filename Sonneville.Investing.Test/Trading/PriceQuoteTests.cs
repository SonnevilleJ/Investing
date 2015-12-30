using System;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class PriceQuoteTests
    {
        [Test]
        [TestCase("a", 5)]
        public void ShouldReturnValuesSpecified(string ticker, int price)
        {
            var dateTime = DateTime.Today;
            var priceQuote = new PriceQuote
            {
                Ticker = ticker,
                PerSharePrice = price,
                DateTime = dateTime
            };

            Assert.AreEqual(ticker, priceQuote.Ticker);
            Assert.AreEqual(price, priceQuote.PerSharePrice);
            Assert.AreEqual(dateTime, priceQuote.DateTime);
        }
    }
}