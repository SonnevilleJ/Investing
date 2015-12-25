using System;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.ShareStrategies;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting.ShareStrategies
{
    [TestFixture]
    public class CannotSellMoreSharesThanOwnedStrategyTests
    {
        private CannotSellMoreSharesThanOwnedStrategy _strategy;

        private Mock<IShareAccount> _shareAccountMock;

        [SetUp]
        public void Setup()
        {
            _shareAccountMock = new Mock<IShareAccount>();

            _strategy = new CannotSellMoreSharesThanOwnedStrategy();
        }

        [Test]
        public void ShouldNotThrowIfSufficientSharesAvailable()
        {
            const string ticker = "DE";
            _shareAccountMock.Setup(account => account.CountHeldShares(ticker)).Returns(10);
            var sell = new Sell(DateTime.Today, ticker, 2.5m, 1, 1);

            _strategy.ProcessTransaction(_shareAccountMock.Object, sell);
            _shareAccountMock.Verify(
                account => account.Deposit(It.Is<IDeposit>(deposit => VerifyDepositForSell(deposit, sell))));
        }

        private static bool VerifyDepositForSell(IDeposit deposit, ISell sell)
        {
            Assert.AreEqual(sell.SettlementDate, deposit.SettlementDate);
            Assert.AreEqual(sell.Amount, deposit.Amount);
            return true;
        }

        [Test]
        public void ShouldThrowIfInsufficientSharesAvailable()
        {
            const string ticker = "DE";
            _shareAccountMock.Setup(account => account.CountHeldShares(ticker)).Returns(0);
            var sell = new Sell(DateTime.Today, ticker, 2.5m, 1, 1);

            Assert.Throws<InvalidOperationException>(() => _strategy.ProcessTransaction(_shareAccountMock.Object, sell));
            _shareAccountMock.Verify(account => account.Deposit(It.IsAny<IDeposit>()), Times.Never());
        }
    }
}