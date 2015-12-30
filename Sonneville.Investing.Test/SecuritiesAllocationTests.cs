using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Sonneville.Investing.Test
{
    [TestFixture]
    public class SecuritiesAllocationTests
    {
        private Dictionary<string, double> _ratesByTicker;
        private SecuritiesAllocation _securitiesAllocation;

        [SetUp]
        public void Setup()
        {
            _ratesByTicker = new Dictionary<string, double>
            {
                {"ticker1", 0.8},
                {"ticker2", 0.2},
            };
            _securitiesAllocation = SecuritiesAllocation.FromDictionary(_ratesByTicker);
        }

        [Test]
        public void ShouldReturnAmountSpecified()
        {
            foreach (var kvp in _ratesByTicker)
            {
                Assert.AreEqual(kvp.Value, _securitiesAllocation.GetAmount(kvp.Key));
            }
        }

        [Test]
        public void ShouldReturnZeroForUnknownTicker()
        {
            Assert.AreEqual(0, _securitiesAllocation.GetAmount("ticker3"));
        }

        [Test]
        public void ShouldEnsureOneHundredPercentAllocation()
        {
            var underAllocated = new Dictionary<string, double>
            {
                {"ticker1", 0.2},
                {"ticker2", 0.2},
            };
            Assert.Throws<InvalidOperationException>(() => SecuritiesAllocation.FromDictionary(underAllocated));

            var overAllocated = new Dictionary<string, double>
            {
                {"ticker1", 0.9},
                {"ticker2", 0.2},
            };
            Assert.Throws<InvalidOperationException>(() => SecuritiesAllocation.FromDictionary(overAllocated));
        }

        [Test]
        public void ShouldEnsureOnlyPositiveAllocations()
        {
            var allocated = new Dictionary<string, double>
            {
                {"ticker1", -0.2},
                {"ticker2", 1.2},
            };
            Assert.Throws<InvalidOperationException>(() => SecuritiesAllocation.FromDictionary(allocated));
        }

        [Test]
        public void ShouldIterate()
        {
            foreach (var kvp in _securitiesAllocation)
            {
                Assert.AreEqual(_ratesByTicker[kvp.Key], kvp.Value);
            }
        }

        [Test]
        public void ShouldReturnEquivalentButNotSameDictionary()
        {
            var returnedDictionary = _securitiesAllocation.ToDictionary();

            Assert.AreNotSame(_ratesByTicker, returnedDictionary);
            CollectionAssert.AreEquivalent(_ratesByTicker, returnedDictionary);
        }
    }
}