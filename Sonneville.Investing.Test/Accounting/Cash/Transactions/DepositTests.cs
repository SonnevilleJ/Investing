﻿using System;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Cash.Transactions;

namespace Sonneville.Investing.Test.Accounting.Cash.Transactions
{
    [TestFixture]
    public class DepositTests
    {
        [Test]
        public void ConstructorTest()
        {
            var dateTime = DateTime.Today;
            const decimal amount = 5m;
            const string memo = "asdf";
            var deposit = new Deposit(dateTime, amount, memo);

            Assert.AreEqual(dateTime, deposit.SettlementDate);
            Assert.AreEqual(amount, deposit.Amount);
            Assert.AreEqual(memo, deposit.Memo);
        }

        [Test]
        public void MemoDefaultsToEmptyString()
        {
            var dateTime = DateTime.Today;
            const decimal amount = 5m;
            var deposit = new Deposit(dateTime, amount);

            Assert.AreEqual(dateTime, deposit.SettlementDate);
            Assert.AreEqual(amount, deposit.Amount);
            Assert.AreEqual(string.Empty, deposit.Memo);
        }
    }
}