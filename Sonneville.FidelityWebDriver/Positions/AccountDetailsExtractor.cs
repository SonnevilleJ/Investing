using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public AccountDetailsExtractor(IPositionDetailsExtractor positionDetailsExtractor, ILog log)
        {
            _positionDetailsExtractor = positionDetailsExtractor;
            _log = log;
        }

        public IEnumerable<IAccountDetails> ExtractAccountDetails(IWebDriver webDriver)
        {
            var accountTypesByAccountNumber = MapAccountNumberToAccountType(webDriver);

            var table = webDriver.FindElements(By.ClassName("p-positions-tbody"))[1];
            var tableRows = table.FindElements(By.TagName("tr")).AsEnumerable();

            using (var e = tableRows.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (IsNewAccountRow(e.Current))
                    {
                        yield return ParseAccountDetails(accountTypesByAccountNumber, e);
                    }
                }
            }
        }

        private IAccountDetails ParseAccountDetails(IReadOnlyDictionary<string, AccountType> accountTypesByAccountNumber, IEnumerator<IWebElement> e)
        {
            var tableRow = e.Current;
            var partialAccountDetails = CreatePartialAccountDetails(tableRow, accountTypesByAccountNumber);
            var positionRows = new List<IWebElement>();

            while (e.MoveNext() && (tableRow = e.Current) != null)
            {
                if (IsPositionRow(tableRow))
                {
                    positionRows.Add(tableRow);
                    continue;
                }

                if (IsTotalRow(tableRow))
                {
                    return CompleteAccountDetails(partialAccountDetails, tableRow, positionRows);
                }
            }
            throw new Exception("oawienvpoqhedonqdp");
        }

        private AccountDetails CreatePartialAccountDetails(IWebElement tableRow, IReadOnlyDictionary<string, AccountType> accountTypesByAccountNumber)
        {
            var accountNumber = ExctractAccountNumber(tableRow);
            _log.Debug($"Starting extraction of details for account {accountNumber}...");
            return new AccountDetails
            {
                Name = ExtractAccountName(tableRow),
                AccountNumber = accountNumber,
                AccountType = accountTypesByAccountNumber[accountNumber],
            };
        }

        private IAccountDetails CompleteAccountDetails(AccountDetails accountDetails, IWebElement tableRow, IEnumerable<IWebElement> positionRows)
        {
            _log.Debug($"Completing extraction of details for account {accountDetails.AccountNumber}...");
            accountDetails.PendingActivity = ReadPendingActivity(tableRow);

            var totalGainSpans = tableRow.FindElements(By.ClassName("magicgrid--stacked-data-value"));
            var trimmedGainText = totalGainSpans[0].Text.Trim();
            accountDetails.TotalGainDollar = ReadTotalDollarGain(trimmedGainText);
            accountDetails.TotalGainPercent = ReadTotalPercentGain(totalGainSpans, trimmedGainText);
            accountDetails.Positions = _positionDetailsExtractor.ExtractPositionDetails(positionRows);
            return accountDetails;
        }

        private static decimal ReadPendingActivity(IWebElement tableRow)
        {
            var pendingActivityDiv = tableRow.FindElement(By.ClassName("magicgrid--total-pending-activity-link-cell"));
            if (!string.IsNullOrWhiteSpace(pendingActivityDiv.Text))
            {
                var rawPendingActivityText = pendingActivityDiv
                    .FindElement(By.ClassName("magicgrid--total-pending-activity-link"))
                    .FindElement(By.ClassName("value"))
                    .Text;
                return NumberParser.ParseDecimal(rawPendingActivityText);
            }

            return default(decimal);
        }

        private static decimal ReadTotalDollarGain(string trimmedGainText)
        {
            if (!string.IsNullOrWhiteSpace(trimmedGainText))
            {
                return NumberParser.ParseDecimal(trimmedGainText);
            }

            return default(decimal);
        }

        private static decimal ReadTotalPercentGain(IReadOnlyList<IWebElement> totalGainSpans, string trimmedGainText)
        {
            var trimmedPercentText = totalGainSpans[1].Text.Trim('%');
            if (!string.IsNullOrWhiteSpace(trimmedGainText))
            {
                return NumberParser.ParseDecimal(trimmedPercentText) / 100m;
            }

            return default(decimal);
        }

        private static Dictionary<string, AccountType> MapAccountNumberToAccountType(IWebDriver webDriver)
        {
            var accountTypes = new Dictionary<AccountType, string>
            {
                {AccountType.InvestmentAccount, "IA"},
                {AccountType.RetirementAccount, "RA"},
                {AccountType.HealthSavingsAccount, "HS"},
                {AccountType.Other, "OA"},
                {AccountType.CreditCard, "CC"},
            }.ToDictionary(
                map => map.Key,
                map => FindWebElementsOfAccountType(webDriver, map.Value)
            ).SelectMany(kvp => kvp.Value.ToDictionary(
                webElement => webElement.Text,
                webElement => kvp.Key)
            ).ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
            );
            return accountTypes;
        }

        private static ReadOnlyCollection<IWebElement> FindWebElementsOfAccountType(IWebDriver webDriver, string classNameToFind)
        {
            return webDriver
                .FindElement(By.ClassName(classNameToFind))
                .FindElements(By.ClassName("account-selector--account-number"));
        }

        private static string ExctractAccountNumber(IWebElement tableRow)
        {
            return tableRow.FindElement(By.ClassName("magicgrid--account-title-description")).Text
                .Replace("†", "");
        }

        private static string ExtractAccountName(IWebElement tableRow)
        {
            return tableRow.FindElement(By.ClassName("magicgrid--account-title-text")).Text
                .Replace("-", "").Trim();
        }

        private static bool IsTotalRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && classes.Contains("magicgrid--total-row");
        }

        private static bool IsPositionRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && (classes.Contains("normal-row") || classes.Contains("content-row"));
        }

        private static bool IsNewAccountRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && classes.Trim().Contains("magicgrid--account-title-row");
        }
    }
}
