using Sonneville.Investing.Trading;
using FidelityAccountType = Sonneville.Fidelity.WebDriver.Data.AccountType;

namespace Sonneville.Fidelity.Shell.FidelityWebDriver
{
    public class AccountTypeMapper
    {
        public AccountType Map(FidelityAccountType accountType)
        {
            switch (accountType)
            {
                case FidelityAccountType.RetirementAccount:
                    return AccountType.RetirementAccount;
                case FidelityAccountType.HealthSavingsAccount:
                    return AccountType.HealthSavingsAccount;
                case FidelityAccountType.InvestmentAccount:
                    return AccountType.InvestmentAccount;
                case FidelityAccountType.CreditCard:
                    return AccountType.CreditCard;
                case FidelityAccountType.Other:
                    return AccountType.Other;
                default:
                    return AccountType.Unknown;
            }
        }
    }
}