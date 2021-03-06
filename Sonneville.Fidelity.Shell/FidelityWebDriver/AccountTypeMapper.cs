using Sonneville.Investing.Trading;
using FidelityAccountType = Sonneville.Investing.Domain.AccountType;

namespace Sonneville.Fidelity.Shell.FidelityWebDriver
{
    public class AccountTypeMapper
    {
        public AccountType MapToInvesting(FidelityAccountType accountType)
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

        public FidelityAccountType MapToFidelity(AccountType accountType)
        {
            switch (accountType)
            {
                case AccountType.RetirementAccount:
                    return FidelityAccountType.RetirementAccount;
                case AccountType.HealthSavingsAccount:
                    return FidelityAccountType.HealthSavingsAccount;
                case AccountType.InvestmentAccount:
                    return FidelityAccountType.InvestmentAccount;
                case AccountType.CreditCard:
                    return FidelityAccountType.CreditCard;
                case AccountType.Other:
                    return FidelityAccountType.Other;
                default:
                    return FidelityAccountType.Unknown;
            }
        }
    }
}