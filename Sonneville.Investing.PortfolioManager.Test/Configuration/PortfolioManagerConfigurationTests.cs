using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.PortfolioManager.Configuration;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.PortfolioManager.Test.Configuration
{
    [TestFixture]
    public class PortfolioManagerConfigurationTests
    {
        [SetUp]
        public void Setup()
        {
            ClearPersistedConfiguration();
        }

        [TearDown]
        public void Teardown()
        {
            ClearPersistedConfiguration();
        }

        [Test]
        public void ShouldInitializeToEmptyListOfAccountTypes()
        {
            var configuration = GetInitializedConfig();

            CollectionAssert.IsEmpty(configuration.InScopeAccountTypes);
        }

        [Test]
        public void ShouldStoreListOfAccountTypes()
        {
            var accountTypes = new HashSet<AccountType>
            {
                AccountType.InvestmentAccount,
                AccountType.RetirementAccount,
            };
            var configuration = GetInitializedConfig();
            configuration.InScopeAccountTypes = accountTypes;
            configuration.Write();

            var portfolioManagerConfiguration = GetInitializedConfig();
            CollectionAssert.AreEquivalent(accountTypes, portfolioManagerConfiguration.InScopeAccountTypes);
        }

        private void ClearPersistedConfiguration()
        {
            var configuration = GetInitializedConfig();

            configuration.InScopeAccountTypes = null;
            configuration.Write();
        }

        private PortfolioManagerConfiguration GetInitializedConfig()
        {
            var configuration = new PortfolioManagerConfiguration();
            configuration.Initialize();
            return configuration;
        }
    }
}