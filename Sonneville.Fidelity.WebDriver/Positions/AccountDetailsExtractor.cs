using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Sonneville.Fidelity.WebDriver.Logging;
using Sonneville.Investing.Domain;

namespace Sonneville.Fidelity.WebDriver.Positions
{
    public interface IAccountDetailsExtractor
    {
        IEnumerable<IAccountDetails> ExtractAccountDetails(IWebDriver webDriver);
    }

    public class AccountDetailsExtractor : IAccountDetailsExtractor
    {
        private readonly IAccountDetailsAggregator _accountDetailsAggregator;
        private readonly ISeleniumWaiter _seleniumWaiter;

        public AccountDetailsExtractor(
            IAccountDetailsAggregator accountDetailsAggregator,
            ISeleniumWaiter seleniumWaiter)
        {
            _accountDetailsAggregator = accountDetailsAggregator;
            _seleniumWaiter = seleniumWaiter;
        }

        public IEnumerable<IAccountDetails> ExtractAccountDetails(IWebDriver webDriver)
        {
            var tableRows = FindAccountDetailsTableRows(webDriver);
            using (var e = tableRows.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (IsNewAccountRow(e.Current))
                    {
                        yield return _accountDetailsAggregator.ParseAccountDetails(e, webDriver);
                    }
                }
            }
        }

        private IEnumerable<IWebElement> FindAccountDetailsTableRows(IWebDriver webDriver)
        {
            return FindAccountDetailsTable(webDriver)
                .FindElements(By.TagName("tr"))
                .AsEnumerable();
        }

        private IWebElement FindAccountDetailsTable(IWebDriver webDriver)
        {
            _seleniumWaiter.WaitUntil(driver => driver.FindElements(By.ClassName("p-positions-tbody")).Count > 1,
                TimeSpan.FromMinutes(1), webDriver);
            return webDriver.FindElements(By.ClassName("p-positions-tbody"))[1];
        }

        private static bool IsNewAccountRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && classes.Trim().Contains("magicgrid--account-title-row");
        }
    }
}