using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.Investing.PortfolioManager.FidelityWebDriver;

namespace Sonneville.Investing.PortfolioManager.Test.FidelityWebDriver
{
    [TestFixture]
    public class AccountTypeMapperTests
    {
        [Test]
        [TestCase(AccountType.HealthSavingsAccount, Trading.AccountType.HealthSavingsAccount)]
        [TestCase(AccountType.InvestmentAccount, Trading.AccountType.InvestmentAccount)]
        [TestCase(AccountType.RetirementAccount, Trading.AccountType.RetirementAccount)]
        [TestCase(AccountType.CreditCard, Trading.AccountType.CreditCard)]
        [TestCase(AccountType.Other, Trading.AccountType.Other)]
        [TestCase(AccountType.Unknown, Trading.AccountType.Unknown)]
        [TestCase(-1, Trading.AccountType.Unknown)]
        public void ShouldMapEnumValues(AccountType fidelityAccountType, Trading.AccountType expectedAccountType)
        {
            var actualAccountType = new AccountTypeMapper().Map(fidelityAccountType);

            Assert.AreEqual(expectedAccountType, actualAccountType);
        }
    }
}