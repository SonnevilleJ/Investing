using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class AccountAllocationTests
    {
        [Test]
        public void ShouldReturnCorrectAmount()
        {
            var accountDictionary = new Dictionary<string, PositionAllocation>
            {
                {
                    "account 1",
                    PositionAllocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {
                            "position a",
                            1
                        }
                    })
                }
            };

            var allocation = AccountAllocation.FromDictionary(accountDictionary);

            Assert.AreEqual(accountDictionary["account 1"], allocation.GetPositionAllocation("account 1"));
        }

        [Test]
        public void ShouldReturnIdenticalDictionary()
        {
            var accountDictionary = new Dictionary<string, PositionAllocation>
            {
                {
                    "account 1",
                    PositionAllocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {
                            "position a",
                            1
                        }
                    })
                }
            };

            var allocation = AccountAllocation.FromDictionary(accountDictionary);
            var dictionary = allocation.ToDictionary();

            CollectionAssert.AreEquivalent(accountDictionary, dictionary);
            Assert.AreNotSame(accountDictionary, dictionary);
        }

        [Test]
        public void ShouldIgnoreChangesToOriginalDictionary()
        {
            var accountDictionary = new Dictionary<string, PositionAllocation>
            {
                {
                    "account 1",
                    PositionAllocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {
                            "position a",
                            1
                        }
                    })
                }
            };

            var allocation = AccountAllocation.FromDictionary(accountDictionary);
            var dictionary = allocation.ToDictionary();

            accountDictionary["account 1"] = PositionAllocation.FromDictionary(new Dictionary<string, decimal>
            {
                {
                    "position B",
                    1
                }
            });

            CollectionAssert.AreNotEquivalent(accountDictionary, dictionary);
            Assert.AreEqual(0, allocation.GetPositionAllocation("account 1").GetPercent("position B"));
        }

        [Test]
        public void ShouldIgnoreChangesToCreatedDictionary()
        {
            var accountDictionary = new Dictionary<string, PositionAllocation>
            {
                {
                    "account 1",
                    PositionAllocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {
                            "position a",
                            1
                        }
                    })
                }
            };

            var allocation = AccountAllocation.FromDictionary(accountDictionary);
            var dictionary = allocation.ToDictionary();
            CollectionAssert.AreEquivalent(accountDictionary, dictionary);

            dictionary["account 1"] = PositionAllocation.FromDictionary(new Dictionary<string, decimal>
            {
                {
                    "position B",
                    1
                }
            });

            CollectionAssert.AreNotEquivalent(accountDictionary, dictionary);
            Assert.AreNotSame(allocation.ToDictionary(), dictionary);
            Assert.AreEqual(0, allocation.GetPositionAllocation("account 1").GetPercent("position B"));
        }
    }
}