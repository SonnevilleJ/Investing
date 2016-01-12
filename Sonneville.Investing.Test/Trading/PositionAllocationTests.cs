using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class PositionAllocationTests
    {
        [Test]
        [TestCase("ticker1", 1, "ticker1", 1)]
        [TestCase("ticker1", 1, "ticker2", 0)]
        public void ShouldReturnCorrectAmount(string allocatedTicker, decimal allocatedPercent, string requestedTicker,
            decimal expectedPercent)
        {
            var positionsDictionary = new Dictionary<string, decimal>
            {
                {
                    allocatedTicker,
                    allocatedPercent
                }
            };

            var allocation = PositionAllocation.FromDictionary(positionsDictionary);

            Assert.AreEqual(expectedPercent, allocation.GetPercent(requestedTicker));
        }

        [Test]
        [TestCase(100, 100, "ticker1")]
        [TestCase(50, 50, "ticker1")]
        [TestCase(2000, 2000, "ticker1")]
        [TestCase(2000, 0, "ticker2")]
        public void DollarTest(decimal dollarsAvailable, decimal expectedDollars, string requestedTicker)
        {
            var positionsDictionary = new Dictionary<string, decimal>
            {
                {
                    "ticker1",
                    1
                },
            };

            var allocation = PositionAllocation.FromDictionary(positionsDictionary);

            Assert.AreEqual(expectedDollars, allocation.GetDollars(requestedTicker, dollarsAvailable));
        }

        [Test]
        public void ShouldReturnIdenticalDictionary()
        {
            var positionsDictionary = new Dictionary<string, decimal>
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

            var allocation = PositionAllocation.FromDictionary(positionsDictionary);
            var dictionary = allocation.ToDictionary();

            CollectionAssert.AreEquivalent(positionsDictionary, dictionary);
            Assert.AreNotSame(positionsDictionary, dictionary);
        }

        [Test]
        public void ShouldIgnoreChangesToOriginalDictionary()
        {
            var positionsDictionary = new Dictionary<string, decimal>
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

            var allocation = PositionAllocation.FromDictionary(positionsDictionary);
            var dictionary = allocation.ToDictionary();

            positionsDictionary.Add("ticker3", 5m);

            CollectionAssert.AreNotEquivalent(positionsDictionary, dictionary);
            Assert.AreEqual(0, allocation.GetPercent("ticker3"));
        }

        [Test]
        public void ShouldIgnoreChangesToCreatedDictionary()
        {
            var positionsDictionary = new Dictionary<string, decimal>
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

            var allocation = PositionAllocation.FromDictionary(positionsDictionary);
            var dictionary = allocation.ToDictionary();
            CollectionAssert.AreEquivalent(positionsDictionary, dictionary);

            dictionary.Add("ticker3", 5m);

            CollectionAssert.AreNotEquivalent(positionsDictionary, dictionary);
            Assert.AreNotSame(allocation.ToDictionary(), dictionary);
            Assert.AreEqual(0, allocation.GetPercent("ticker3"));
        }
    }
}