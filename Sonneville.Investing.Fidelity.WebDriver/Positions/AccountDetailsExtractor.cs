using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Sonneville.Investing.Domain;

namespace Sonneville.Investing.Fidelity.WebDriver.Positions
{
    public interface IAccountDetailsExtractor
    {
        IEnumerable<IAccountDetails> ExtractAccountDetails(IWebDriver webDriver);
    }

    public class AccountDetailsExtractor : IAccountDetailsExtractor
    {
        private readonly IAccountDetailsAggregator _accountDetailsAggregator;

        public AccountDetailsExtractor(IAccountDetailsAggregator accountDetailsAggregator)
        {
            _accountDetailsAggregator = accountDetailsAggregator;
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

        private static IEnumerable<IWebElement> FindAccountDetailsTableRows(IWebDriver webDriver)
        {
            return webDriver.FindElements(By.ClassName("p-positions-tbody"))[1]
                .FindElements(By.TagName("tr"))
                .AsEnumerable();
        }

        private static bool IsNewAccountRow(IWebElement tableRow)
        {
            var classes = tableRow.GetAttribute("class");
            return !string.IsNullOrWhiteSpace(classes)
                   && classes.Trim().Contains("magicgrid--account-title-row");
        }
    }
}