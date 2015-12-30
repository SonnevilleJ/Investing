using System.Collections.Generic;
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
        public void ShouldReturnAmountSpecified()
        {
            Assert.AreEqual(0.8, _calculator.GetAllocation("ticker1", _positions));
            Assert.AreEqual(0.2, _calculator.GetAllocation("ticker2", _positions));
        }

        [Test]
        public void ShouldReturnZeroForUnknownTicker()
        {
            Assert.AreEqual(0, _calculator.GetAllocation("ticker3", _positions));
        }
    }
}