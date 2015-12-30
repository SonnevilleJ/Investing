using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class SecuritiesAllocationTests
    {
        private Dictionary<string, decimal> _ratesByTicker;
        private SecuritiesAllocation _securitiesAllocation;

        [SetUp]
        public void Setup()
        {
            _ratesByTicker = new Dictionary<string, decimal>
            {
                {"ticker1", 0.8m},
                {"ticker2", 0.2m},
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
            var underAllocated = new Dictionary<string, decimal>
            {
                {"ticker1", 0.2m},
                {"ticker2", 0.2m},
            };
            Assert.Throws<InvalidOperationException>(() => SecuritiesAllocation.FromDictionary(underAllocated));

            var overAllocated = new Dictionary<string, decimal>
            {
                {"ticker1", 0.9m},
                {"ticker2", 0.2m},
            };
            Assert.Throws<InvalidOperationException>(() => SecuritiesAllocation.FromDictionary(overAllocated));
        }

        [Test]
        public void ShouldEnsureOnlyPositiveAllocations()
        {
            var allocated = new Dictionary<string, decimal>
            {
                {"ticker1", -0.2m},
                {"ticker2", 1.2m},
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