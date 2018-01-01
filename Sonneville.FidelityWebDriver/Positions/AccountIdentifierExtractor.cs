using log4net;
using OpenQA.Selenium;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface IAccountIdentifierExtractor
    {
        string ExctractAccountNumber(IWebElement tableRow);

        string ExtractAccountName(IWebElement tableRow);
    }

    public class AccountIdentifierExtractor : IAccountIdentifierExtractor
    {
        private readonly ILog _log;

        public AccountIdentifierExtractor(ILog log)
        {
            _log = log;
        }

        public string ExctractAccountNumber(IWebElement tableRow)
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
    }
}