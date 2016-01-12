using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class TradingAccountBalanceCalculatorTests
    {
        [Test]
        public void ShouldSumSingleAccount()
        {
            var tradingAccount = new TradingAccount
            {
                Positions = new List<Position>
                {
                    new Position
                    {
                        Shares = 1,
                        PerSharePrice = 10
                    },
                    new Position
                    {
                        Shares = 2,
                        PerSharePrice = 3
                    },
                }
            };

            var calculator = new TradingAccountBalanceCalculator();

            var balance = calculator.CalculateBalance(tradingAccount);

            Assert.AreEqual(16, balance);
        }

        [Test]
        public void ShouldSumMultipleAccounts()
        {
            var tradingAccounts = new List<TradingAccount>
            {
                new TradingAccount
                {
                    Positions = new List<Position>
                    {
                        new Position
                        {
                            Shares = 1,
                            PerSharePrice = 10
                        },
                        new Position
                        {
                            Shares = 2,
                            PerSharePrice = 3
                        },
                    }
                },
                new TradingAccount
                {
                    Positions = new List<Position>
                    {
                        new Position
                        {
                            Shares = 10,
                            PerSharePrice = 5
                        },
                        new Position
                        {
                            Shares = 3,
                            PerSharePrice = 7
                        },
                    }
                }
            };

            var calculator = new TradingAccountBalanceCalculator();

            var balance = calculator.CalculateBalance(tradingAccounts);

            Assert.AreEqual(87, balance);
        }
    }
}