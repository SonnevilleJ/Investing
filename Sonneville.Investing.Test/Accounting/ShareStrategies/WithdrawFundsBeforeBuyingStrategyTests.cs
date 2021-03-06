﻿using System;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Cash.Transactions;
using Sonneville.Investing.Accounting.Securities;
using Sonneville.Investing.Accounting.Securities.Transactions;
using Sonneville.Investing.Accounting.ShareStrategies;

namespace Sonneville.Investing.Test.Accounting.ShareStrategies
{
    [TestFixture]
    public class WithdrawFundsBeforeBuyingStrategyTests
    {
        private WithdrawFundsBeforeBuyingStrategy _strategy;

        [SetUp]
        public void Setup()
        {
            _strategy = new WithdrawFundsBeforeBuyingStrategy();
        }

        [Test]
        public void ShouldInitiateWithdrawalToMeetFundingRequirements()
        {
            var buy = new Buy(DateTime.Today, "DE", 1, 50m, 7.95m, "my first share!");
            var shareAccountMock = new Mock<IShareAccount>();
            shareAccountMock.Setup(cashAccount => cashAccount.Withdraw(It.IsAny<IWithdrawal>()))
                .Callback<IWithdrawal>(withdrawal => VerifyWithdrawlCreatedForBuy(buy, withdrawal));

            _strategy.ProcessTransaction(shareAccountMock.Object, buy);

            shareAccountMock.Verify(cashAccount => cashAccount.Withdraw(It.IsAny<IWithdrawal>()));
        }

        private static void VerifyWithdrawlCreatedForBuy(IBuy buy, IWithdrawal withdrawal)
        {
            Assert.AreEqual(DateTime.Today, withdrawal.SettlementDate.Date);
            Assert.AreEqual(-buy.Amount, withdrawal.Amount);
            Assert.AreEqual(
                $"Automatic withdrawal of {buy.Amount:C} for buying {buy.Ticker}",
                withdrawal.Memo);
        }
    }
}