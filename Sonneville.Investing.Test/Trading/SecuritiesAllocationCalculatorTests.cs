using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class SecuritiesAllocationCalculatorTests
    {
        private List<Position> _positions;
        private SecuritiesAllocationCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _positions = new List<Position>
            {
                new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
            };

            _calculator = new SecuritiesAllocationCalculator();
        }

        [Test]
        public void ShouldReturnAllocatedPercentage()
        {
            Assert.AreEqual(0.8, _calculator.CalculateAllocation("ticker1", _positions));
            Assert.AreEqual(0.2, _calculator.CalculateAllocation("ticker2", _positions));
        }

        [Test]
        public void ShouldReturnAllocationByTicker()
        {
            var allocationsByTicker = _calculator.CalculateAllocations(_positions);

            Assert.AreEqual(2, allocationsByTicker.Count());
            Assert.AreEqual(0.8, allocationsByTicker["ticker1"]);
            Assert.AreEqual(0.2, allocationsByTicker["ticker2"]);
        }

        [Test]
        public void ShouldReturnZeroForUnknownTicker()
        {
            Assert.AreEqual(0, _calculator.CalculateAllocation("ticker3", _positions));
        }

        [Test]
        public void ShouldThrowWhenPassedMultiplePositionsWithSameTicker()
        {
            var positions = new List<Position>
            {
                new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
                new Position {Ticker = "ticker2", PerSharePrice = 10, Shares = 25},
            };

            Assert.Throws<ArgumentException>(() => _calculator.CalculateAllocation("ticker1", positions));
            Assert.Throws<ArgumentException>(() => _calculator.CalculateAllocation("ticker2", positions));
        }
    }
}