using System;
using NUnit.Framework;
using Sonneville.Investing.PortfolioManager.FidelityWebDriver;
using WebDriverPosition = Sonneville.FidelityWebDriver.Data.Position;

namespace Sonneville.Investing.PortfolioManager.Test.FidelityWebDriver
{
    [TestFixture]
    public class PositionMapperTests
    {
        [Test]
        public void ShouldMapProperties()
        {
            var extractedPosition = new WebDriverPosition
            {
                Ticker = "a",
                Quantity = 10,
                LastPrice = 15,
            };

            var mappedPosition = new PositionMapper().Map(extractedPosition);

            Assert.AreEqual("a", mappedPosition.Ticker);
            Assert.AreEqual(DateTime.Today, mappedPosition.DateTime);
            Assert.AreEqual(extractedPosition.Quantity, mappedPosition.Shares);
            Assert.AreEqual(extractedPosition.LastPrice, mappedPosition.PerSharePrice);
        }
    }
}