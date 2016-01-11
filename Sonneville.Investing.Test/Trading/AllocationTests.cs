using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class AllocationTests
    {
        [Test]
        [TestCase("ticker1", 1, "ticker1", 1)]
        [TestCase("ticker1", 1, "ticker2", 0)]
        public void ShouldReturnCorrectAmount(string allocatedTicker, decimal allocatedPercent, string requestedTicker, decimal expectedPercent)
        {
            var accountDictionary = new Dictionary<string, decimal>
            {
                {
                    allocatedTicker,
                    allocatedPercent
                }
            };

            var allocation = Allocation.FromDictionary(accountDictionary);

            Assert.AreEqual(expectedPercent, allocation.GetPercent(requestedTicker));
        }

        [Test]
        [TestCase(0.4, 0.5)]
        [TestCase(0.6, 0.5)]
        public void ShouldThrowIfAnyTickersDoNotSumTo100Percent(decimal percent1, decimal percent2)
        {
            var accountDictionary = new Dictionary<string, decimal>
            {
                {
                    "ticker1",
                    percent1
                },
                {
                    "ticker2",
                    percent2
                },
            };

            Assert.Throws<ArgumentException>(() => Allocation.FromDictionary(accountDictionary));
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        public void ShouldThrowIfAnyTickersAreZeroOrLess(decimal percent1, decimal percent2)
        {
            var accountDictionary = new Dictionary<string, decimal>
            {
                {
                    "ticker1",
                    percent1
                },
                {
                    "ticker2",
                    percent2
                },
            };

            Assert.Throws<ArgumentException>(() => Allocation.FromDictionary(accountDictionary));
        }

        [Test]
        [TestCase(100, 100, "ticker1")]
        [TestCase(50, 50, "ticker1")]
        [TestCase(2000, 2000, "ticker1")]
        [TestCase(2000, 0, "ticker2")]
        public void DollarTest(decimal dollarsAvailable, decimal expectedDollars, string requestedTicker)
        {
            var accountDictionary = new Dictionary<string, decimal>
            {
                {
                    "ticker1",
                    1
                },
            };

            var allocation = Allocation.FromDictionary(accountDictionary);

            Assert.AreEqual(expectedDollars, allocation.GetDollars(requestedTicker, dollarsAvailable));
        }

        [Test]
        public void ShouldReturnIdenticalDictionary()
        {
            var accountDictionary = new Dictionary<string, decimal>
            {
                {
                    "ticker1",
                    0.5m
                },
                {
                    "ticker2",
                    0.5m
                },
            };

            var allocation = Allocation.FromDictionary(accountDictionary);
            var dictionary = allocation.ToDictionary();

            CollectionAssert.AreEquivalent(accountDictionary, dictionary);
            Assert.AreNotSame(accountDictionary, dictionary);
        }

        [Test]
        public void ShouldIgnoreChangesToOriginalDictionary()
        {
            var accountDictionary = new Dictionary<string, decimal>
            {
                {
                    "ticker1",
                    0.5m
                },
                {
                    "ticker2",
                    0.5m
                },
            };

            var allocation = Allocation.FromDictionary(accountDictionary);
            var dictionary = allocation.ToDictionary();

            accountDictionary.Add("ticker3", 5m);

            CollectionAssert.AreNotEquivalent(accountDictionary, dictionary);
            Assert.AreEqual(0, allocation.GetPercent("ticker3"));
        }
    }
}