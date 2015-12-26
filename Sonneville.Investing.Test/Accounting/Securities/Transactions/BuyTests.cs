using System;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Test.Accounting.Securities.Transactions
{
    [TestFixture]
    public class BuyTests
    {
        [Test]
        public void ConstructorTest()
        {
            var settlementDate = new DateTime(2015, 12, 23);
            const string ticker = "DE";
            const decimal shares = 100.0m;
            const decimal price = 5.6m;
            const decimal commission = 7.8m;
            const string memo = "I'm sure this will go up!";
            var buy = new Buy(settlementDate, ticker, shares, price, commission, memo);

            Assert.AreEqual(settlementDate, buy.SettlementDate);
            Assert.AreEqual(ticker, buy.Ticker);
            Assert.AreEqual(shares, buy.Shares);
            Assert.AreEqual(price, buy.PerSharePrice);
            Assert.AreEqual(commission, buy.Commission);
            Assert.AreEqual(567.8m, buy.Amount);
            Assert.AreEqual(memo, buy.Memo);
        }

        [Test]
        public void MemoShouldDefaultToEmptyString()
        {
            var settlementDate = new DateTime(2015, 12, 23);
            const string ticker = "DE";
            const decimal shares = 100.0m;
            const decimal price = 5.6m;
            const decimal commission = 7.8m;
            var buy = new Buy(settlementDate, ticker, shares, price, commission);

            Assert.AreEqual(string.Empty, buy.Memo);
        }
    }
}