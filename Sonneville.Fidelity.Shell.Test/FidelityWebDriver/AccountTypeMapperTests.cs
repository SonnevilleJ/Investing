using NUnit.Framework;
using Sonneville.Fidelity.Shell.FidelityWebDriver;
using Sonneville.Investing.Domain;

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
        public void ShouldMapToInvesting(AccountType fidelityAccountType, Investing.Trading.AccountType investingAccountType)
        {
            var actualAccountType = new AccountTypeMapper().MapToInvesting(fidelityAccountType);

            Assert.AreEqual(investingAccountType, actualAccountType);
        }
        
        [Test]
        [TestCase(AccountType.HealthSavingsAccount, Investing.Trading.AccountType.HealthSavingsAccount)]
        [TestCase(AccountType.InvestmentAccount, Investing.Trading.AccountType.InvestmentAccount)]
        [TestCase(AccountType.RetirementAccount, Investing.Trading.AccountType.RetirementAccount)]
        [TestCase(AccountType.CreditCard, Investing.Trading.AccountType.CreditCard)]
        [TestCase(AccountType.Other, Investing.Trading.AccountType.Other)]
        [TestCase(AccountType.Unknown, Investing.Trading.AccountType.Unknown)]
        [TestCase(AccountType.Unknown, -1)]
        public void ShouldMapToFidelity(AccountType fidelityAccountType, Investing.Trading.AccountType investingAccountType)
        {
            var actualAccountType = new AccountTypeMapper().MapToFidelity(investingAccountType);

            Assert.AreEqual(fidelityAccountType, actualAccountType);
        }
    }
}