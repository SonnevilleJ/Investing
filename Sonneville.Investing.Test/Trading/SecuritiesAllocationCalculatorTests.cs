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
            Assert.AreEqual(0.8, _calculator.CalculateAllocation(_positions[0], _positions));
            Assert.AreEqual(0.2, _calculator.CalculateAllocation(_positions[1], _positions));
        }

        [Test]
        public void ShouldReturnAllocationByTicker()
        {
            var allocationsByTicker = _calculator.CalculateAllocations(_positions);

            Assert.AreEqual(2, allocationsByTicker.Count());
            Assert.AreEqual(0.8, allocationsByTicker[_positions[0]]);
            Assert.AreEqual(0.2, allocationsByTicker[_positions[1]]);
        }

        [Test]
        public void ShouldThrowIfPositionNotInList()
        {
            var positions = new List<Position>
            {
                new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
                new Position {Ticker = "ticker2", PerSharePrice = 10, Shares = 25},
            };

            var position = new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200};
            Assert.Throws<ArgumentException>(() => _calculator.CalculateAllocation(position, positions));
        }
    }
}