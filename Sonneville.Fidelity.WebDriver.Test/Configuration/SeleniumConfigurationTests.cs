using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Fidelity.WebDriver.Configuration;
using Sonneville.Investing.Trading;

namespace Sonneville.Fidelity.WebDriver.Test.Configuration
{
    [TestFixture]
    public class SeleniumConfigurationTests
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