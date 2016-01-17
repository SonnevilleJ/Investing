using System.Collections.Generic;
using System.IO.IsolatedStorage;
using NUnit.Framework;
using Sonneville.Investing.PortfolioManager.Configuration;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.PortfolioManager.Test.Configuration
{
    [TestFixture]
    public class PortfolioManagerConfigurationTests
    {
        private IsolatedStorageFile _isolatedStore;

        [SetUp]
        public void Setup()
        {
            _isolatedStore = IsolatedStorageFile.GetUserStoreForAssembly();
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
            var configuration = PortfolioManagerConfiguration.Initialize(_isolatedStore);

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
            var configuration = PortfolioManagerConfiguration.Initialize(_isolatedStore);
            configuration.InScopeAccountTypes = accountTypes;
            configuration.Write();

            var portfolioManagerConfiguration = PortfolioManagerConfiguration.Initialize(_isolatedStore);
            CollectionAssert.AreEquivalent(accountTypes, portfolioManagerConfiguration.InScopeAccountTypes);
        }

        private void ClearPersistedConfiguration()
        {
            foreach (var fileName in _isolatedStore.GetFileNames())
            {
                _isolatedStore.DeleteFile(fileName);
            }
        }
    }
}