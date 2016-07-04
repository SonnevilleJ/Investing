using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.Fidelity.Shell.FidelityWebDriver;

namespace Sonneville.Fidelity.Shell.Test.FidelityWebDriver
{
    [TestFixture]
    public class AccountTypeMapperTests
    {
        [Test]
        [TestCase(AccountType.HealthSavingsAccount, Investing.Trading.AccountType.HealthSavingsAccount)]
        [TestCase(AccountType.InvestmentAccount, Investing.Trading.AccountType.InvestmentAccount)]
        [TestCase(AccountType.RetirementAccount, Investing.Trading.AccountType.RetirementAccount)]
        [TestCase(AccountType.CreditCard, Investing.Trading.AccountType.CreditCard)]
        [TestCase(AccountType.Other, Investing.Trading.AccountType.Other)]
        [TestCase(AccountType.Unknown, Investing.Trading.AccountType.Unknown)]
        [TestCase(-1, Investing.Trading.AccountType.Unknown)]
        public void ShouldMapEnumValues(AccountType fidelityAccountType, Investing.Trading.AccountType expectedAccountType)
        {
            var actualAccountType = new AccountTypeMapper().Map(fidelityAccountType);

            Assert.AreEqual(expectedAccountType, actualAccountType);
        }
    }
}