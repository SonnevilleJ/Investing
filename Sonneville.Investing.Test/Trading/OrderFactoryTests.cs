using System;
using NUnit.Framework;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Test.Trading
{
    [TestFixture]
    public class OrderFactoryTests
    {
        [Test]
        public void ShouldAssignProperties()
        {
            const string ticker = "asdf";
            const TransactionType transactionType = TransactionType.Buy;
            var creationDate = DateTime.Now;
            const decimal pricePerShare = 5.5m;
            const decimal shares = 10m;

            var buy = new OrderFactory().Create(ticker, transactionType, creationDate, shares, pricePerShare);

            Assert.AreEqual(ticker, buy.Ticker);
            Assert.AreEqual(transactionType, buy.TransactionType);
            Assert.AreEqual(creationDate, buy.CreationDate);
            Assert.AreEqual(shares, buy.Shares);
            Assert.AreEqual(pricePerShare, buy.PricePerShare);
        }
    }
}