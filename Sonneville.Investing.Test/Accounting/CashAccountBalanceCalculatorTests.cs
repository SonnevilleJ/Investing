using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting
{
    public class CashAccountBalanceCalculatorTests
    {
        private CashAccountBalanceCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new CashAccountBalanceCalculator();
        }

        [Test]
        public void BalanceShouldNotReflectFutureTransactions()
        {
            var cashTransactions = new List<ICashTransaction>
            {
                new Deposit(new DateTime(2010, 1, 16), 500.00m),
                new Deposit(new DateTime(2010, 1, 17), 100.00m),
                new Deposit(new DateTime(2010, 1, 18), 1000.00m),
                new Withdrawal(new DateTime(2015, 12, 22), 200m)
            };

            var accountBalance = _calculator.CalculateBalance(new DateTime(2010, 1, 18).AddTicks(-1), cashTransactions);

            Assert.AreEqual(600m, accountBalance);
        }

        [Test]
        public void BalanceShouldReflectWithdrawnFunds()
        {
            var cashTransactions = new List<ICashTransaction>
            {
                new Deposit(new DateTime(2010, 1, 16), 500.00m),
                new Withdrawal(new DateTime(2015, 12, 22), 200m)
            };

            var accountBalance = _calculator.CalculateBalance(new DateTime(2015, 12, 22), cashTransactions);

            Assert.AreEqual(300m, accountBalance);
        }

        [Test]
        public void BalanceNotAffectedByTransactionEntryDate()
        {
            var cashTransactions = new List<ICashTransaction>
            {
                new Withdrawal(new DateTime(2010, 1, 16), 200m)
            };

            var accountBalance = _calculator.CalculateBalance(new DateTime(2010, 1, 16), cashTransactions);

            Assert.AreEqual(-200m, accountBalance);
        }
    }
}