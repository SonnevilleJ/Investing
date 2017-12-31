using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface IAccountSummariesExtractor
    {
        IEnumerable<IAccountSummary> ExtractAccountSummaries(IWebDriver webDriver);
    }

    public class AccountSummariesExtractor : IAccountSummariesExtractor
    {
        private static readonly Dictionary<string, AccountType> GroupIdsToAccountTypes = new Dictionary<string, AccountType>
        {
            {"IA", AccountType.InvestmentAccount},
            {"RA", AccountType.RetirementAccount},
            {"HS", AccountType.HealthSavingsAccount},
            {"OA", AccountType.Other},
            {"CC", AccountType.CreditCard},
        };

        public IEnumerable<IAccountSummary> ExtractAccountSummaries(IWebDriver webDriver)
        {
            var accountGroupDivs = webDriver.FindElements(By.ClassName("account-selector--group-container"));

            foreach (var accountGroupDiv in accountGroupDivs)
            {
                var accountType = GroupIdsToAccountTypes[accountGroupDiv.GetAttribute("data-group-id")];
                var accountDivs = accountGroupDiv.FindElements(By.ClassName("js-account"));

                foreach (var accountDiv in accountDivs)
                {
                    var accountNumber = accountDiv.GetAttribute("data-acct-number");
                    var accountName = accountDiv.GetAttribute("data-acct-name");
                    var value = accountType == AccountType.CreditCard
                        ? NumberParser.ParseDouble(accountDiv.GetAttribute("data-acct-balance"))
                        : NumberParser.ParseDouble(accountDiv.GetAttribute("data-most-recent-value"));
                    yield return new AccountSummary
                    {
                        AccountNumber = accountNumber,
                        AccountType = accountType,
                        Name = accountName,
                        MostRecentValue = value
                    };
                }
            }
        }
    }
}