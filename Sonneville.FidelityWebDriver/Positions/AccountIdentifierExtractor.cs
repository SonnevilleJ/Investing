using log4net;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface IAccountIdentifierExtractor
    {
        string ExtractAccountNumber(IWebElement tableRow);

        string ExtractAccountName(IWebElement tableRow);

        AccountType ExtractAccountType(IWebElement accountDefinitionRow, IWebDriver webDriver);
    }

    public class AccountIdentifierExtractor : IAccountIdentifierExtractor
    {
        private readonly ILog _log;
        private readonly IAccountTypesMapper _accountTypesMapper;

        public AccountIdentifierExtractor(ILog log, IAccountTypesMapper accountTypesMapper)
        {
            _accountTypesMapper = accountTypesMapper;
            _log = log;
        }

        public string ExtractAccountNumber(IWebElement tableRow)
        {
            var accountNumber = tableRow.FindElement(By.ClassName("magicgrid--account-title-description")).Text
                .Replace("†", "");
            _log.Debug($"Starting extraction of details for account {accountNumber}...");
            return accountNumber;
        }

        public string ExtractAccountName(IWebElement tableRow)
        {
            return tableRow.FindElement(By.ClassName("magicgrid--account-title-text")).Text
                .Replace("-", "").Trim();
        }

        public AccountType ExtractAccountType(IWebElement accountDefinitionRow, IWebDriver webDriver)
        {
            var accountTypesByAccountNumber = _accountTypesMapper.ReadAccountTypes(webDriver);

            if (!accountTypesByAccountNumber.TryGetValue(ExtractAccountNumber(accountDefinitionRow), out var accountType))
            {
                _log.Warn($"NOT able to parse unknown account type for account: {ExtractAccountNumber(accountDefinitionRow)}");
                return AccountType.Unknown;
            }

            return accountType;
        }
    }
}
