using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.Investing.PortfolioManager.Configuration;
using Sonneville.Investing.PortfolioManager.FidelityWebDriver;
using Sonneville.Investing.Trading;
using Sonneville.Utilities.Configuration;
using AccountType = Sonneville.Investing.Trading.AccountType;

namespace Sonneville.Investing.PortfolioManager.Test
{
    [TestFixture]
    public class AccountRebalancerTests
    {
        private AccountRebalancer _accountRebalancer;
        private Mock<IPositionsManager> _positionsManagerMock;
        private Mock<ISecuritiesAllocationCalculator> _allocationCalculatorMock;
        private Mock<IAccountMapper> _accountMapperMock;
        private List<TradingAccount> _tradingAccounts;
        private PortfolioManagerConfiguration _portfolioManagerConfiguration;

        [SetUp]
        public void Setup()
        {
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

            _portfolioManagerConfiguration = new ConfigStore(IsolatedStorageFile.GetUserStoreForAssembly()).Get<PortfolioManagerConfiguration>();
            _portfolioManagerConfiguration.InScopeAccountTypes = new HashSet<AccountType>
            {
                AccountType.InvestmentAccount,
                AccountType.HealthSavingsAccount,
                AccountType.RetirementAccount,
            };

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