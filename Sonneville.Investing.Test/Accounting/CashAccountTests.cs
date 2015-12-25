using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.CashStrategies;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting
{
    [TestFixture]
    public class CashAccountTests
    {
        private CashAccount _cashAccount;

        private Mock<ICashTransactionStrategy<IDeposit>> _depositValidatorMock;

        private Mock<ICashTransactionStrategy<IWithdrawal>> _withdrawalValidatorMock;

        [SetUp]
        public void Setup()
        {
            _depositValidatorMock = SetupCashTransactionValidator<IDeposit>();

            _withdrawalValidatorMock = SetupCashTransactionValidator<IWithdrawal>();

            _cashAccount = new CashAccount(_depositValidatorMock.Object, _withdrawalValidatorMock.Object);
        }

        private static Mock<ICashTransactionStrategy<T>> SetupCashTransactionValidator<T>() where T : ICashTransaction
        {
            var validatorMock = new Mock<ICashTransactionStrategy<T>>();
            validatorMock.Setup(validator => validator.ThrowIfInvalid(It.IsAny<T>(), It.IsAny<ICashAccount>()))
                .Callback<T, ICashAccount>(
                    (transaction, cashAccount) =>
                    {
                        Assert.IsFalse(cashAccount.CashTransactions.Contains(transaction));
                    });

            return validatorMock;
        }

        [Test]
        public void DepositTest()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), -500.00m);

            var cashAccount = _cashAccount.Deposit(deposit);

            _depositValidatorMock.Verify(validator => validator.ThrowIfInvalid(deposit, _cashAccount));
            Assert.AreSame(_cashAccount, cashAccount);
            Assert.IsTrue(_cashAccount.CashTransactions.Contains(deposit));
        }

        [Test]
        public void WithdrawalTest()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            _cashAccount.Deposit(deposit);
            var withdrawal = new Withdrawal(new DateTime(2015, 12, 22), -200m);

            var cashAccount = _cashAccount.Withdraw(withdrawal);

            _withdrawalValidatorMock.Verify(validator => validator.ThrowIfInvalid(withdrawal, _cashAccount));
            Assert.AreSame(_cashAccount, cashAccount);
            Assert.IsTrue(_cashAccount.CashTransactions.Contains(withdrawal));
        }

        [Test]
        public void TransactionsAreReadonly()
        {
            var transactions = _cashAccount.CashTransactions;

            Assert.IsInstanceOf<IReadOnlyCollection<ICashTransaction>>(transactions);
        }
    }
}