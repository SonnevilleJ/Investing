using System;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Accounting;
using Sonneville.Investing.Accounting.Strategies;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Test.Accounting.Strategies
{
    [TestFixture]
    public class WithdrawFundsBeforeBuyingStrategyTests
    {
        private WithdrawFundsBeforeBuyingStrategy _strategy;

        private Mock<ICashAccount> _cashAccountMock;

        [SetUp]
        public void Setup()
        {
            _cashAccountMock = new Mock<ICashAccount>();

            _strategy = new WithdrawFundsBeforeBuyingStrategy();
        }

        [Test]
        public void ShouldInitiateWithdrawalToMeetFundingRequirements()
        {
            var buy = new Buy(DateTime.Today, "DE", 1, 50m, 7.95m, "my first share!");
            _cashAccountMock.Setup(cashAccount => cashAccount.Withdraw(It.IsAny<IWithdrawal>()))
                .Callback<IWithdrawal>(withdrawal => VerifyBuyCreatedForWithdrawal(buy, withdrawal));

            _strategy.ProcessBuy(_cashAccountMock.Object, buy);

            _cashAccountMock.Verify(cashAccount => cashAccount.Withdraw(It.IsAny<IWithdrawal>()));
        }

        private static void VerifyBuyCreatedForWithdrawal(Buy buy, IWithdrawal withdrawal)
        {
            Assert.AreEqual(DateTime.Today, withdrawal.SettlementDate.Date);
            Assert.AreEqual(-buy.Amount, withdrawal.Amount);
            Assert.AreEqual(
                $"Automatic withdrawal of {buy.Amount:C} for buying {buy.Ticker}",
                withdrawal.Memo);
        }
    }
}