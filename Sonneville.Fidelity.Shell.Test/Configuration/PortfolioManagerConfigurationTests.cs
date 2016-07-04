using System.Collections.Generic;
using System.IO.IsolatedStorage;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Configuration;
using Sonneville.Investing.Trading;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Fidelity.Shell.Test.Configuration
{
    [TestFixture]
    public class PortfolioManagerConfigurationTests
    {
        private ConfigStore _configStore;

        [SetUp]
        public void Setup()
        {
            _configStore = new ConfigStore(IsolatedStorageFile.GetUserStoreForAssembly());
            _configStore.Clear();
        }

        [TearDown]
        public void Teardown()
        {
            _configStore.Clear();
        }

        [Test]
        public void ShouldInitializeToEmptyListOfAccountTypes()
        {
            var configuration = _configStore.Get<PortfolioManagerConfiguration>();

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
            var configuration = _configStore.Get<PortfolioManagerConfiguration>();
            configuration.InScopeAccountTypes = accountTypes;
            configuration.Write();

            var portfolioManagerConfiguration = _configStore.Get<PortfolioManagerConfiguration>();
            CollectionAssert.AreEquivalent(accountTypes, portfolioManagerConfiguration.InScopeAccountTypes);
        }
    }
}