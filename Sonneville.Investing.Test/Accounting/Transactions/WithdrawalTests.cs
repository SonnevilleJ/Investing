﻿using System;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting.Transactions
{
    [TestFixture]
    public class WithdrawalTests
    {
        [Test]
        public void TestConstruction()
        {
            var dateTime = DateTime.Today;
            const decimal amount = 5m;
            var deposit = new Withdrawal(dateTime, amount);

            Assert.AreEqual(dateTime, deposit.SettlementDate);
            Assert.AreEqual(amount, deposit.Amount);
        }
    }
}