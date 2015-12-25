using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.ShareStrategies;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting
{
    [TestFixture]
    public class ShareAccountTests
    {
        private ShareAccount _shareAccount;

        private Mock<ICashAccount> _cashAccountMock;

        private Mock<IShareTransactionStrategy<Buy>> _buyStrategyMock;

        private Mock<IShareTransactionStrategy<Sell>> _sellStrategyMock;

        [SetUp]
        public void Setup()
        {
            _cashAccountMock = new Mock<ICashAccount>();

            _buyStrategyMock = new Mock<IShareTransactionStrategy<Buy>>();

            _sellStrategyMock = new Mock<IShareTransactionStrategy<Sell>>();

            _shareAccount = new ShareAccount(_cashAccountMock.Object, _buyStrategyMock.Object, _sellStrategyMock.Object);
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

        [Test]
        public void BuyShouldDelegateToStrategyBeforeCommittingBuy()
        {
            var buy = new Buy(DateTime.Today, "DE", 1, 50m, 7.95m, "my first share!");
            _buyStrategyMock.Setup(processor => processor.ProcessTransaction(_shareAccount, buy))
                .Callback<ICashAccount, Buy>((account, b) =>
                {
                    Assert.AreSame(_shareAccount, account);
                    Assert.IsFalse(_shareAccount.ShareTransactions.Contains(b));
                });

            var shareAccount = _shareAccount.Buy(buy);

            Assert.IsTrue(_shareAccount.ShareTransactions.Contains(buy));
            Assert.AreSame(_shareAccount, shareAccount);
            Assert.AreEqual(buy.Shares, _shareAccount.CountHeldShares(buy.Ticker));
            _buyStrategyMock.Verify(processor => processor.ProcessTransaction(_shareAccount, buy));
        }

        [Test]
        public void CountHeldSharesShouldCountBoughtShares()
        {
            const string ticker = "DE";
            var buy = new Buy(DateTime.Today, ticker, 1, 50m, 7.95m, "my first share!");
            var sell = new Sell(DateTime.Today, ticker, 1, 500m, 7.95m, "helluvadeal");
            _sellStrategyMock.Setup(processor => processor.ProcessTransaction(_shareAccount, sell))
                .Callback<ICashAccount, Sell>((account, s) =>
                {
                    Assert.AreSame(_shareAccount, account);
                    Assert.IsFalse(_shareAccount.ShareTransactions.Contains(s));
                });

            var shareAccount = _shareAccount.Buy(buy)
                .Sell(sell);

            Assert.IsTrue(_shareAccount.ShareTransactions.Contains(sell));
            Assert.AreSame(_shareAccount, shareAccount);
            Assert.AreEqual(0, _shareAccount.CountHeldShares(ticker));
            _sellStrategyMock.Verify(processor => processor.ProcessTransaction(_shareAccount, sell));
        }
    }
}