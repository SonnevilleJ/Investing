using System;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Test.Accounting.Securities.Transactions
{
    [TestFixture]
    public class SellTests
    {
        [Test]
        public void ConstructorTest()
        {
            var settlementDate = new DateTime(2015, 12, 23);
            const string ticker = "DE";
            const decimal shares = 100.0m;
            const decimal price = 5.6m;
            const decimal commission = 7.8m;
            const string memo = "I'm cashing in on this!";
            var sell = new Sell(settlementDate, ticker, shares, price, commission, memo);

            Assert.AreEqual(settlementDate, sell.SettlementDate);
            Assert.AreEqual(ticker, sell.Ticker);
            Assert.AreEqual(-shares, sell.Shares);
            Assert.AreEqual(price, sell.PerSharePrice);
            Assert.AreEqual(commission, sell.Commission);
            Assert.AreEqual(-552.2m, sell.Amount);
            Assert.AreEqual(memo, sell.Memo);
        }

        [Test]
        public void MemoShouldDefaultToEmptyString()
        {
            var settlementDate = new DateTime(2015, 12, 23);
            const string ticker = "DE";
            const decimal shares = 100.0m;
            const decimal price = 5.6m;
            const decimal commission = 7.8m;
            var sell = new Sell(settlementDate, ticker, shares, price, commission);

            Assert.AreEqual(string.Empty, sell.Memo);
        }
    }
}