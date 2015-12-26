using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.Cash.Transactions;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;
using Sonneville.Investing.Accounting.ShareStrategies;

namespace Sonneville.Investing.Test.Accounting.ShareStrategies
{
    [TestFixture]
    public class CannotSellMoreSharesThanOwnedStrategyTests
    {
        private CannotSellMoreSharesThanOwnedStrategy _strategy;

        private Mock<IShareAccount> _shareAccountMock;
        private Mock<IHeldSharesCalculator> _shareAccountHeldSharesCalculatorMock;
        private IReadOnlyCollection<IShareTransaction> _shareTransactions;

        [SetUp]
        public void Setup()
        {
            _shareAccountMock = new Mock<IShareAccount>();
            _shareTransactions = _shareAccountMock.Object.ShareTransactions;

            _shareAccountHeldSharesCalculatorMock = new Mock<IHeldSharesCalculator>();

            _strategy = new CannotSellMoreSharesThanOwnedStrategy(_shareAccountHeldSharesCalculatorMock.Object);
        }

        [Test]
        public void ShouldNotThrowIfSufficientSharesAvailable()
        {
            const string ticker = "DE";
            _shareAccountHeldSharesCalculatorMock.Setup(
                calculator => calculator.CountHeldShares(ticker, _shareTransactions))
                .Returns(10);
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
            _shareAccountHeldSharesCalculatorMock.Setup(
                calculator => calculator.CountHeldShares(ticker, _shareTransactions)).Returns(0);
            var sell = new Sell(DateTime.Today, ticker, 2.5m, 1, 1);

            Assert.Throws<InvalidOperationException>(() => _strategy.ProcessTransaction(_shareAccountMock.Object, sell));
            _shareAccountMock.Verify(account => account.Deposit(It.IsAny<IDeposit>()), Times.Never());
        }
    }
}