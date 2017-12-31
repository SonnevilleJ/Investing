using System.Collections.Generic;
using log4net;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface IAccountDetailsExtractor
    {
        IEnumerable<IAccountDetails> ExtractAccountDetails(IWebDriver webDriver);
    }

    public class AccountDetailsExtractor : IAccountDetailsExtractor
    {
        private readonly IPositionDetailsExtractor _positionDetailsExtractor;
        private readonly ILog _log;
        private readonly Dictionary<AccountType, string> _accountTypeToGroupIds;

        public AccountDetailsExtractor(IPositionDetailsExtractor positionDetailsExtractor, ILog log)
        {
            _positionDetailsExtractor = positionDetailsExtractor;
            _log = log;

            _accountTypeToGroupIds = new Dictionary<AccountType, string>
            {
                {AccountType.InvestmentAccount, "IA"},
                {AccountType.RetirementAccount, "RA"},
                {AccountType.HealthSavingsAccount, "HS"},
                {AccountType.Other, "OA"},
                {AccountType.CreditCard, "CC"},
            };
        }

        public IEnumerable<IAccountDetails> ExtractAccountDetails(IWebDriver webDriver)
        {
            var accountTypes = new Dictionary<string, AccountType>();
            foreach (var accountTypeToGroupId in _accountTypeToGroupIds)
            {
                var groupDiv = webDriver.FindElement(By.ClassName(accountTypeToGroupId.Value));
                var accountSpans = groupDiv.FindElements(By.ClassName("account-selector--account-number"));
                foreach (var accountSpan in accountSpans)
                {
                    accountTypes.Add(accountSpan.Text, accountTypeToGroupId.Key);
                }
            }

            var table = webDriver.FindElements(By.ClassName("p-positions-tbody"))[1];
            var tableRows = table.FindElements(By.TagName("tr"));

            var collectingPositionRows = false;

            List<IWebElement> positionRows = null;
            AccountDetails accountDetails = null;

            var results = new List<IAccountDetails>();
            foreach (var tableRow in tableRows)
            {
                if (collectingPositionRows)
                {
                    if (IsPositionRow(tableRow))
                    {
                        positionRows.Add(tableRow);
                        continue;
                    }
                    if (IsTotalRow(tableRow))
                    {
                        var pendingActivityDiv =
                            tableRow.FindElement(By.ClassName("magicgrid--total-pending-activity-link-cell"));
                        if (!string.IsNullOrWhiteSpace(pendingActivityDiv.Text))
                        {
                            var rawPendingActivityText = pendingActivityDiv
                                .FindElement(By.ClassName("magicgrid--total-pending-activity-link"))
                                .FindElement(By.ClassName("value"))
                                .Text;
                            accountDetails.PendingActivity = NumberParser.ParseDecimal(rawPendingActivityText);
                        }

                        var totalGainSpans = tableRow.FindElements(By.ClassName("magicgrid--stacked-data-value"));
                        var trimmedGainText = totalGainSpans[0].Text.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmedGainText))
                        {
                            accountDetails.TotalGainDollar = NumberParser.ParseDecimal(trimmedGainText);
                        }
                        var trimmedPercentText = totalGainSpans[1].Text.Trim('%');
                        if (!string.IsNullOrWhiteSpace(trimmedGainText))
                        {
                            accountDetails.TotalGainPercent = NumberParser.ParseDecimal(trimmedPercentText)/100m;
                        }

                        collectingPositionRows = false;
                    }
                }
                if (IsNewAccountRow(tableRow))
                {
                    if (accountDetails != null)
                    {
                        results.Add(PrepareAccountDetails(accountDetails, positionRows));
                    }
                    positionRows = new List<IWebElement>();
                    var accountNumber = ExctractAccountNumber(tableRow).Replace("†", "");
                    _log.Debug($"Starting to extract details for account {accountNumber}...");
                    accountDetails = new AccountDetails
                    {
                        Name = ExtractAccountName(tableRow),
                        AccountNumber = accountNumber,
                        AccountType = accountTypes[accountNumber],
                    };
                    collectingPositionRows = true;
                }
            }
            results.Add(PrepareAccountDetails(accountDetails, positionRows));
            return results;
        }

        private AccountDetails PrepareAccountDetails(AccountDetails accountDetails,
            IEnumerable<IWebElement> positionRows)
        {
            _log.Debug($"Completed parsing details for {accountDetails.AccountNumber}.");
            accountDetails.Positions = _positionDetailsExtractor.ExtractPositionDetails(positionRows);
            return accountDetails;
        }

        private static string ExctractAccountNumber(IWebElement tableRow)
        {
            return tableRow.FindElement(By.ClassName("magicgrid--account-title-description")).Text;
        }

        private static string ExtractAccountName(IWebElement tableRow)
        {
            return tableRow.FindElement(By.ClassName("magicgrid--account-title-text")).Text
                .Replace("-", "").Trim();
        }

        private bool IsTotalRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && classes.Contains("magicgrid--total-row");
        }

        private bool IsPositionRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && (classes.Contains("normal-row") || classes.Contains("content-row"));
        }

        private bool IsNewAccountRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && classes.Trim().Contains("magicgrid--account-title-row");
        }
    }
}