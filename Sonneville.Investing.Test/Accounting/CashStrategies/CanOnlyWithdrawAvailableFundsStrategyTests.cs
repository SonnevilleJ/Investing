using System;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.Cash;
using Sonneville.Investing.Accounting.Cash.Transactions;
using Sonneville.Investing.Accounting.CashStrategies;

namespace Sonneville.Investing.Test.Accounting.CashStrategies
{
    [TestFixture]
    public class CanOnlyWithdrawAvailableFundsStrategyTests
    {
        private CanOnlyWithdrawAvailableFundsStrategy _strategy;

        private Mock<ICashAccount> _cashAccountMock;

        private decimal _accountMinimum;

        private Mock<ICashAccountBalanceCalculator> _balanceCalculatorMock;

        [SetUp]
        public void Setup()
        {
            _cashAccountMock = new Mock<ICashAccount>();

            _balanceCalculatorMock = new Mock<ICashAccountBalanceCalculator>();
            _accountMinimum = 5m;

            _strategy = new CanOnlyWithdrawAvailableFundsStrategy(_balanceCalculatorMock.Object, _accountMinimum);
        }

        [Test]
        public void ShouldNotThrowWhenWithdrawingLessThanAccountBalance()
        {
            var withdrawal = new Withdrawal(DateTime.Today, -1);
            _balanceCalculatorMock.Setup(calculator =>
                calculator.CalculateBalance(withdrawal.SettlementDate, _cashAccountMock.Object.CashTransactions))
                .Returns(10m)
                .Verifiable();

            _strategy.ThrowIfInvalid(withdrawal, _cashAccountMock.Object);

            _balanceCalculatorMock.Verify();
        }

        [Test]
        public void ShouldThrowWhenWithdrawingMoreThanAccountBalance()
        {
            var withdrawal = new Withdrawal(DateTime.Today, -7);
            _balanceCalculatorMock.Setup(calculator =>
                calculator.CalculateBalance(withdrawal.SettlementDate, _cashAccountMock.Object.CashTransactions))
                .Returns(10m);

            Assert.Throws<InvalidOperationException>(() => _strategy.ThrowIfInvalid(withdrawal, _cashAccountMock.Object));
        }
    }
}