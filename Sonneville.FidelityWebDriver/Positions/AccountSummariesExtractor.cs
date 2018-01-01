using System.Collections.Generic;
using System.Linq;
using log4net;
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
        private readonly ILog _log;

        public AccountSummariesExtractor(ILog log)
        {
            _log = log;
        }

        public IEnumerable<IAccountSummary> ExtractAccountSummaries(IWebDriver webDriver)
        {
            var accountGroupDivs = webDriver.FindElements(By.ClassName("account-selector--group-container"));

            var groupIdsToAccountTypes = AccountTypesMapper.CodesForKnownAccountTypes
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            foreach (var accountGroupDiv in accountGroupDivs)
            {
                var accountTypeAttribute = accountGroupDiv.GetAttribute("data-group-id");

                if (!groupIdsToAccountTypes.TryGetValue(accountTypeAttribute, out var accountType))
                {
                    _log.Warn($"NOT able to parse unknown account type: {accountTypeAttribute}");
                    continue;
                }

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
