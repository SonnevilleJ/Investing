using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Configuration;
using Sonneville.Fidelity.Shell.FidelityWebDriver;
using Sonneville.Fidelity.WebDriver.Data;
using Sonneville.Fidelity.WebDriver.Positions;
using Sonneville.Investing.Trading;
using AccountType = Sonneville.Investing.Trading.AccountType;

namespace Sonneville.Fidelity.Shell.Test
{
    [TestFixture]
    public class AccountRebalancerTests
    {
        private AccountRebalancer _accountRebalancer;
        private Mock<IPositionsManager> _positionsManagerMock;
        private Mock<ISecuritiesAllocationCalculator> _allocationCalculatorMock;
        private Mock<IAccountMapper> _accountMapperMock;
        private List<TradingAccount> _tradingAccounts;
        private SeleniumConfiguration _portfolioManagerConfiguration;

        [SetUp]
        public void Setup()
        {
            _portfolioManagerConfiguration = new SeleniumConfiguration();
            var accountDetails = new List<IAccountDetails>
            {
                new AccountDetails
                {
                    Positions = new List<IPosition>()
                }
            };

            _positionsManagerMock = new Mock<IPositionsManager>();
            _positionsManagerMock.Setup(manager => manager.GetAccountDetails()).Returns(accountDetails);

            _tradingAccounts = Enum.GetValues(typeof (AccountType))
                .Cast<AccountType>()
                .Select(type => new TradingAccount {AccountType = type})
                .ToList();

            _accountMapperMock = new Mock<IAccountMapper>();
            _accountMapperMock.Setup(mapper => mapper.Map(accountDetails)).Returns(_tradingAccounts);

            var validAccountTypes = _portfolioManagerConfiguration.InScopeAccountTypes;
            var tradingAccounts = _tradingAccounts.Where(account => validAccountTypes.Contains(account.AccountType))
                .ToList();

            _allocationCalculatorMock = new Mock<ISecuritiesAllocationCalculator>();
            _allocationCalculatorMock.Setup(calculator => calculator.CalculateAccountAllocation(
                It.Is<IReadOnlyList<TradingAccount>>(tas => ValidateTradingAccounts(tas, tradingAccounts))))
                .Verifiable();

            _accountRebalancer = new AccountRebalancer(_positionsManagerMock.Object, _accountMapperMock.Object,
                _allocationCalculatorMock.Object, _portfolioManagerConfiguration);
        }

        [Test]
        public void DisposeShouldNotThrow()
        {
            _accountRebalancer.Dispose();
            _accountRebalancer.Dispose();

            _positionsManagerMock.Verify(manager => manager.Dispose());
        }

        [Test]
        public void RunShouldNotThrow()
        {
            _accountRebalancer.RebalanceAccounts();

            _allocationCalculatorMock.Verify();
        }

        private bool ValidateTradingAccounts(IEnumerable<TradingAccount> actualTradingAccounts,
            IEnumerable<TradingAccount> expectedTradingAccounts)
        {
            CollectionAssert.AreEquivalent(expectedTradingAccounts, actualTradingAccounts);
            return true;
        }
    }
}