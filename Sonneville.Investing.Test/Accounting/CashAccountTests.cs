using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting
{
    [TestFixture]
    public class CashAccountTests
    {
        private CashAccount _cashAccount;

        [SetUp]
        public void Setup()
        {
            _cashAccount = new CashAccount();
        }

        [Test]
        public void TransactionsShouldBeFluent()
        {
            var deposit1 = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            var deposit2 = new Deposit(new DateTime(2010, 1, 17), 500.00m);
            var withdrawal1 = new Withdrawal(new DateTime(2015, 12, 22), 200m);
            var withdrawal2 = new Withdrawal(new DateTime(2015, 12, 23), 200m);
            _cashAccount.Deposit(deposit1)
                .Withdraw(withdrawal1)
                .Deposit(deposit2)
                .Withdraw(withdrawal2);

            var transactions = _cashAccount.CashTransactions;

            CollectionAssert.AreEquivalent(new List<ICashTransaction> {deposit1, withdrawal1, deposit2, withdrawal2}, transactions);
        }

        [Test]
        public void CannotDepositNegativeFunds()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), -500.00m);

            Assert.Throws<InvalidOperationException>(() => _cashAccount.Deposit(deposit));
        }

        [Test]
        public void CannotWithdrawPositiveFunds()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            _cashAccount.Deposit(deposit);
            var withdrawal = new Withdrawal(new DateTime(2015, 12, 22), -200m);

            Assert.Throws<InvalidOperationException>(() => _cashAccount.Withdraw(withdrawal));
        }

        [Test]
        public void CannotWithdrawZeroFunds()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            _cashAccount.Deposit(deposit);
            var withdrawal = new Withdrawal(new DateTime(2015, 12, 22), 0m);

            Assert.Throws<InvalidOperationException>(() => _cashAccount.Withdraw(withdrawal));
        }

        [Test]
        public void TransactionsAreReadonly()
        {
            var transactions = _cashAccount.CashTransactions;

            Assert.IsInstanceOf<IReadOnlyCollection<ICashTransaction>>(transactions);
        }
    }
}