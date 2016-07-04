using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Fidelity.Shell.Test
{
    [TestFixture]
    public class AllocationDifferencerTests
    {
        [Test]
        public void ShouldDiffPercentages_PositionAllocation()
        {
            var minuend = PositionAllocation.FromDictionary(new Dictionary<string, decimal>
            {
                {"a", 0.25m},
                {"b", 0.5m},
                {"c", 0.25m},
                {"d", 0.25m},
            });
            var subtrahend = PositionAllocation.FromDictionary(new Dictionary<string, decimal>
            {
                {"a", 0.5m},
                {"b", 0.25m},
                {"c", 0.25m},
                {"e", 0.25m},
            });

            var difference = new AllocationDifferencer().CalculateDifference(minuend, subtrahend).ToDictionary();

            Assert.AreEqual(-0.25m, difference.Single(kvp => kvp.Key == "a").Value);
            Assert.AreEqual(0.25m, difference.Single(kvp => kvp.Key == "b").Value);
            Assert.AreEqual(0, difference.Single(kvp => kvp.Key == "c").Value);
            Assert.AreEqual(0.25m, difference.Single(kvp => kvp.Key == "d").Value);
            Assert.AreEqual(-0.25m, difference.Single(kvp => kvp.Key == "e").Value);
        }

        [Test]
        public void ShouldDiffPercentages_AccountAllocation()
        {
            var minuend = AccountAllocation.FromDictionary(new Dictionary<string, PositionAllocation>
            {
                {
                    "account 1",
                    PositionAllocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {"a", 0.25m},
                        {"b", 0.5m},
                        {"c", 0.25m},
                        {"d", 0.25m},
                    })
                },
                {
                    "account 2",
                    PositionAllocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {"a", 0.25m},
                        {"b", 0.5m},
                        {"c", 0.25m},
                    })
                },
            });

            var subtrahend = AccountAllocation.FromDictionary(new Dictionary<string, PositionAllocation>
            {
                {
                    "account 1",
                    PositionAllocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {"a", 0.5m},
                        {"b", 0.25m},
                        {"c", 0.25m},
                    })
                },
                {
                    "account 2",
                    PositionAllocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {"a", 0.5m},
                        {"b", 0.25m},
                        {"c", 0.25m},
                        {"e", 0.25m},
                    })
                },
            });

            var accountDifference = new AllocationDifferencer().CalculateDifference(minuend, subtrahend);
            var account1Allocation = accountDifference.ToDictionary()["account 1"];
            var account1PositionDifference = account1Allocation.ToDictionary();

            Assert.AreEqual(-0.25m, account1PositionDifference.Single(kvp => kvp.Key == "a").Value);
            Assert.AreEqual(0.25m, account1PositionDifference.Single(kvp => kvp.Key == "b").Value);
            Assert.AreEqual(0, account1PositionDifference.Single(kvp => kvp.Key == "c").Value);
            Assert.AreEqual(0.25m, account1PositionDifference.Single(kvp => kvp.Key == "d").Value);

            var account2Allocation = accountDifference.ToDictionary()["account 2"];
            var account2PositionDifference = account2Allocation.ToDictionary();

            Assert.AreEqual(-0.25m, account2PositionDifference.Single(kvp => kvp.Key == "a").Value);
            Assert.AreEqual(0.25m, account2PositionDifference.Single(kvp => kvp.Key == "b").Value);
            Assert.AreEqual(0m, account2PositionDifference.Single(kvp => kvp.Key == "c").Value);
            Assert.AreEqual(-0.25m, account2PositionDifference.Single(kvp => kvp.Key == "e").Value);
        }
    }
}