using System;
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
        public void ShouldReturnCorrectAmount(string allocatedTicker, decimal allocatedPercent, string requestedTicker, decimal expectedPercent)
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
        [TestCase(0.5, 0.501)]
        [TestCase(0.5, 0.499)]
        public void ShouldThrowIfAnyTickersDoNotSumTo100Percent(decimal percent1, decimal percent2)
        {
            var positionsDictionary = new Dictionary<string, decimal>
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

            Assert.Throws<ArgumentException>(() => PositionAllocation.FromDictionary(positionsDictionary));
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        public void ShouldThrowIfAnyPercentagesAreInvalid(decimal percent1, decimal percent2)
        {
            var positionsDictionary = new Dictionary<string, decimal>
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

            Assert.Throws<ArgumentException>(() => PositionAllocation.FromDictionary(positionsDictionary));
        }

        [Test]
        [TestCase(0.001)]
        [TestCase(0.999)]
        public void ShouldNotThrowOnValidPercent(decimal value)
        {
            Assert.DoesNotThrow(() => PositionAllocation.ForMultiAccount(new Dictionary<string, decimal>
            {
                {
                    "position a",
                    value
                }
            }));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1.1)]
        [TestCase(-1)]
        public void ShouldThrowIfPositionCreatedWithInvalidPercent(decimal value)
        {
            var positionDictionary = new Dictionary<string, decimal>
            {
                {
                    "position a",
                    value
                }
            };
            Assert.Throws<ArgumentException>(() => { PositionAllocation.ForMultiAccount(positionDictionary); });
        }

        [Test]
        [TestCase(0.001)]
        [TestCase(0.999)]
        public void ShouldNotThrowIfPositionCreatedWithLessThan100Percent(decimal value)
        {
            var positionDictionary = new Dictionary<string, decimal>
            {
                {
                    "position a",
                    value
                }
            };
            Assert.DoesNotThrow(() => PositionAllocation.ForMultiAccount(positionDictionary));
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