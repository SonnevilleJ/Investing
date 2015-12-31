using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class TradingAccountTests
    {
        [Test]
        public void ShouldReturnValuesSpecified()
        {
            const string accountid = "accountId";
            const decimal pendingFunds = 5;
            var positions = new List<Position>();

            var tradingAccount = new TradingAccount
            {
                AccountId = accountid,
                PendingFunds = pendingFunds,
                Positions = positions,
            };

            Assert.AreEqual(accountid, tradingAccount.AccountId);
            Assert.AreEqual(pendingFunds, tradingAccount.PendingFunds);
            Assert.AreSame(positions, tradingAccount.Positions);
        }
    }
}