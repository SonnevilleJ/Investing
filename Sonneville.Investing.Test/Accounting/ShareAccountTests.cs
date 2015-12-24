using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting
{
    [TestFixture]
    public class ShareAccountTests
    {
        private ShareAccount _shareAccount;

        private Mock<ICashAccount> _cashAccountMock;

        [SetUp]
        public void Setup()
        {
            _cashAccountMock = new Mock<ICashAccount>();
            _shareAccount = new ShareAccount(_cashAccountMock.Object);
        }

        [Test]
        public void DepositsAreStoredInCashAccount()
        {
            var deposit = new Deposit(DateTime.Today, 1234.5m);

            var shareAccount = _shareAccount.Deposit(deposit);

            _cashAccountMock.Verify(cashAccount => cashAccount.Deposit(deposit));
            Assert.AreSame(_shareAccount, shareAccount);
        }

        [Test]
        public void WithdrawalsAreStoredInCashAccount()
        {
            var withdrawal = new Withdrawal(DateTime.Today, 123.4m);

            var shareAccount = _shareAccount.Withdraw(withdrawal);
            
            _cashAccountMock.Verify(cashAccount => cashAccount.Withdraw(withdrawal));
            Assert.AreSame(_shareAccount, shareAccount);
        }

        [Test]
        public void CashTransactionsShouldIncludeThoseFromCashAccount()
        {
            var cashTransactions = new List<ICashTransaction> {new Deposit(DateTime.Today, 1234.5m)};
            _cashAccountMock.Setup(cashAccount => cashAccount.CashTransactions)
                .Returns(cashTransactions.AsReadOnly());

            var actual = _shareAccount.CashTransactions;

            CollectionAssert.AreEquivalent(cashTransactions, actual);
        }
    }
}