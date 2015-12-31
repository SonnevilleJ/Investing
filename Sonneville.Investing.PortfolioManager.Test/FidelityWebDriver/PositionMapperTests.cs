﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.Investing.PortfolioManager.FidelityWebDriver;
using TradingPosition = Sonneville.Investing.Trading.Position;
using WebDriverPosition = Sonneville.FidelityWebDriver.Data.Position;

namespace Sonneville.Investing.PortfolioManager.Test.FidelityWebDriver
{
    [TestFixture]
    public class PositionMapperTests
    {
        [Test]
        public void ShouldMapPosition()
        {
            var extractedPosition = new WebDriverPosition
            {
                Ticker = "a",
                Quantity = 10,
                LastPrice = 15,
            };

            var mappedPosition = new PositionMapper().Map(extractedPosition);

            AssertMapping(extractedPosition, mappedPosition);
        }

        [Test]
        public void ShouldMapPositions()
        {
            var webDriverPositions = new List<WebDriverPosition>
            {
                new WebDriverPosition
                {
                    Ticker = "a",
                    Quantity = 10,
                    LastPrice = 15,
                },
                new WebDriverPosition
                {
                    Ticker = "b",
                    Quantity = 20,
                    LastPrice = 17,
                },
                new WebDriverPosition
                {
                    Ticker = "c",
                    Quantity = 15,
                    LastPrice = 12,
                },
            };

            var mappedPositions = new PositionMapper().Map(webDriverPositions).ToDictionary(p => p.Ticker, p => p);

            foreach (var unmappedPosition in webDriverPositions)
            {
                AssertMapping(unmappedPosition, mappedPositions[unmappedPosition.Ticker]);
            }
        }

        private static void AssertMapping(IPosition unmappedPosition, TradingPosition mappedPosition)
        {
            Assert.AreEqual(unmappedPosition.Ticker, mappedPosition.Ticker);
            Assert.AreEqual(DateTime.Today, mappedPosition.DateTime);
            Assert.AreEqual(unmappedPosition.Quantity, mappedPosition.Shares);
            Assert.AreEqual(unmappedPosition.LastPrice, mappedPosition.PerSharePrice);
        }
    }
}