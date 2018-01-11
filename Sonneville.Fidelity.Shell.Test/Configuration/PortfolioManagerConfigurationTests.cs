using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Configuration;
using Sonneville.Investing.Trading;

namespace Sonneville.Fidelity.Shell.Test.Configuration
{
    [TestFixture]
    public class PortfolioManagerConfigurationTests
    {
        [Test]
        public void ShouldInitializeToEmptyListOfAccountTypes()
        {
            var configuration = new SeleniumConfiguration();

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
            var configuration = new SeleniumConfiguration();
            configuration.InScopeAccountTypes = accountTypes;

            CollectionAssert.AreEquivalent(accountTypes, configuration.InScopeAccountTypes);
        }
    }
}