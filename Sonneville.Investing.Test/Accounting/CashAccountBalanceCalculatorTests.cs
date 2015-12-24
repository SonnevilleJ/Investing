using System;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting
{
    public class CashAccountBalanceCalculatorTests
    {
        private ICashAccount _cashAccount;

        private CashAccountBalanceCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _cashAccount = new CashAccount();
            
            _calculator = new CashAccountBalanceCalculator();
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

            var accountBalance = _calculator.CalculateBalance(deposit3.SettlementDate.AddTicks(-1), _cashAccount);

            Assert.AreEqual(600m, accountBalance);
        }

        [Test]
        public void BalanceShouldReflectWithdrawnFunds()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            _cashAccount.Deposit(deposit);
            var withdrawal = new Withdrawal(new DateTime(2015, 12, 22), 200m);
            _cashAccount.Withdraw(withdrawal);

            var accountBalance = _calculator.CalculateBalance(withdrawal.SettlementDate, _cashAccount);

            Assert.AreEqual(300m, accountBalance);
        }

        [Test]
        public void BalanceNotAffectedByTransactionEntryDate()
        {
            var deposit = new Deposit(new DateTime(2010, 1, 16), 500.00m);
            _cashAccount.Deposit(deposit);
            var withdrawal = new Withdrawal(new DateTime(1776, 7, 4), 200m);
            _cashAccount.Withdraw(withdrawal);

            var accountBalance = _calculator.CalculateBalance(deposit.SettlementDate, _cashAccount);

            Assert.AreEqual(300m, accountBalance);
        }
    }
}