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
        public void BalanceShouldReflectDepositedFunds()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            _cashAccount.Deposit(deposit);

            var accountBalance = _cashAccount.CalculateBalance(deposit.SettlementDate);

            Assert.AreEqual(500m, accountBalance);
        }

        [Test]
        public void FluentTransactions()
        {
            var deposit1 = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            var deposit2 = new Deposit(new DateTime(2010, 1, 17), 100.00m);
            var withdrawal1 = new Withdrawal(new DateTime(2015, 12, 22), 200m);
            var withdrawal2 = new Withdrawal(new DateTime(2015, 12, 23), 100m);
            _cashAccount.Deposit(deposit1)
                .Deposit(deposit2)
                .Withdraw(withdrawal1)
                .Withdraw(withdrawal2);

            var accountBalance = _cashAccount.CalculateBalance(withdrawal2.SettlementDate);

            Assert.AreEqual(300m, accountBalance);
        }

        [Test]
        public void BalanceShouldNotReflectFutureTransactions()
        {
            var deposit1 = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            var deposit2 = new Deposit(new DateTime(2010, 1, 17), 100.00m);
            var deposit3 = new Deposit(new DateTime(2010, 1, 18), 1000.00m);
            var withdrawal = new Withdrawal(new DateTime(2015, 12, 22), 200m);
            _cashAccount.Deposit(deposit1)
                .Deposit(deposit2)
                .Deposit(deposit3)
                .Withdraw(withdrawal);

            var accountBalance = _cashAccount.CalculateBalance(deposit3.SettlementDate.AddTicks(-1));

            Assert.AreEqual(600m, accountBalance);
        }

        [Test]
        public void CannotDepositNegativeFunds()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), -500.00m);

            Assert.Throws<InvalidOperationException>(() => _cashAccount.Deposit(deposit));
        }

        [Test]
        public void BalanceShouldReflectWithdrawnFunds()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            _cashAccount.Deposit(deposit);
            var withdrawal = new Withdrawal(new DateTime(2015, 12, 22), 200m);
            _cashAccount.Withdraw(withdrawal);

            var accountBalance = _cashAccount.CalculateBalance(withdrawal.SettlementDate);

            Assert.AreEqual(300m, accountBalance);
        }

        [Test]
        public void BalanceShouldBeAgnosticToSettlementDates()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            _cashAccount.Deposit(deposit);
            var withdrawal = new Withdrawal(new DateTime(1776, 7, 4), 200m);
            _cashAccount.Withdraw(withdrawal);

            var accountBalance = _cashAccount.CalculateBalance(deposit.SettlementDate);

            Assert.AreEqual(300m, accountBalance);
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
            var transactions = _cashAccount.Transactions;

            Assert.IsInstanceOf<IReadOnlyCollection<ICashTransaction>>(transactions);
        }
    }
}