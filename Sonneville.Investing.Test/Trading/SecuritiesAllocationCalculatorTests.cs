using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class SecuritiesAllocationCalculatorTests
    {
        [Test]
        public void ShouldReturnAllocationOfAllPositionsForOnePosition()
        {
            var positions = new List<Position>
            {
                new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
            };

            Assert.AreEqual(0.8, new SecuritiesAllocationCalculator().CalculateAllocation(positions[0], positions));
            Assert.AreEqual(0.2, new SecuritiesAllocationCalculator().CalculateAllocation(positions[1], positions));
        }

        [Test]
        public void ShouldReturnAllocationOfAllPositionsForEachPosition()
        {
            var positions = new List<Position>
            {
                new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
            };

            var allocationsByPosition = new SecuritiesAllocationCalculator().CalculateAllocations(positions);

            Assert.AreEqual(2, allocationsByPosition.Count());
            Assert.AreEqual(0.8, allocationsByPosition[positions[0]]);
            Assert.AreEqual(0.2, allocationsByPosition[positions[1]]);
        }

        [Test]
        public void ShouldReturnAllocationOfAllAccountsForOnePosition()
        {
            var accounts = new List<TradingAccount>
            {
                CreateTradingAccount("account1", new List<Position>
                {
                    new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                    new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
                }),
                CreateTradingAccount("account2", new List<Position>
                {
                    new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                    new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
                }),
            };

            var allocation = new SecuritiesAllocationCalculator().CalculateAllocation(accounts[0].Positions[0], accounts);

            Assert.AreEqual(0.4, allocation);
        }

        [Test]
        public void ShouldReturnAllocationOfAllAccountsForEachPosition()
        {
            var accounts = new List<TradingAccount>
            {
                CreateTradingAccount("account1", new List<Position>
                {
                    new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                    new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
                }),
                CreateTradingAccount("account2", new List<Position>
                {
                    new Position {Ticker = "ticker1", PerSharePrice = 7, Shares = 150},
                    new Position {Ticker = "ticker2", PerSharePrice = 10, Shares = 30},
                }),
            };

            var allocation = new SecuritiesAllocationCalculator().CalculateAllocation(accounts);

            var totalValue = accounts.Sum(account => account.Positions.Sum(position => position.Value));
            foreach (var account in accounts)
            {
                foreach (var position in account.Positions)
                {
                    Assert.AreEqual(position.Value/totalValue, allocation[account][position]);
                }
            }
        }

        [Test]
        public void ShouldThrowIfPositionNotInListOfPositions()
        {
            var positions = new List<Position>
            {
                new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
                new Position {Ticker = "ticker2", PerSharePrice = 10, Shares = 25},
            };

            var position = new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200};
            var calculator = new SecuritiesAllocationCalculator();
            Assert.Throws<KeyNotFoundException>(() => calculator.CalculateAllocation(position, positions));
        }

        [Test]
        public void ShouldThrowIfPositionNotInListOfAccounts()
        {
            var accounts = new List<TradingAccount>
            {
                CreateTradingAccount("account1", new List<Position>
                {
                    new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                    new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
                }),
                CreateTradingAccount("account2", new List<Position>
                {
                    new Position {Ticker = "ticker1", PerSharePrice = 4, Shares = 200},
                    new Position {Ticker = "ticker2", PerSharePrice = 5, Shares = 40},
                }),
            };

            var calculator = new SecuritiesAllocationCalculator();
            Assert.Throws<KeyNotFoundException>(() => calculator.CalculateAllocation(new Position(), accounts));
        }

        private static TradingAccount CreateTradingAccount(string accountId, IList<Position> positions)
        {
            return new TradingAccount
            {
                AccountId = accountId,
                Positions = positions
            };
        }
    }
}