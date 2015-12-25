using System;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting.Transactions
{
    [TestFixture]
    public class WithdrawalTests
    {
        [Test]
        public void ConstructorTest()
        {
            var dateTime = DateTime.Today;
            const decimal amount = 5m;
            const string memo = "asdf";
            var deposit = new Withdrawal(dateTime, amount, memo);

            Assert.AreEqual(dateTime, deposit.SettlementDate);
            Assert.AreEqual(-amount, deposit.Amount);
            Assert.AreEqual(memo, deposit.Memo);
        }

        [Test]
        public void MemoShouldDefaultToEmptyString()
        {
            var dateTime = DateTime.Today;
            const decimal amount = 5m;
            var deposit = new Withdrawal(dateTime, amount);

            Assert.AreEqual(dateTime, deposit.SettlementDate);
            Assert.AreEqual(-amount, deposit.Amount);
            Assert.AreEqual(string.Empty, deposit.Memo);
        }
    }
}