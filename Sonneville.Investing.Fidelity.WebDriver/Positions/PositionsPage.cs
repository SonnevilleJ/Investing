using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.Investing.Domain;

namespace Sonneville.Investing.Fidelity.WebDriver.Positions
{
    public interface IPositionsPage : IPage
    {
        IEnumerable<IAccountSummary> GetAccountSummaries();

        IEnumerable<IAccountDetails> GetAccountDetails();
    }

    public class PositionsPage : IPositionsPage
    {
        private readonly IAccountDetailsExtractor _accountDetailsExtractor;
        private readonly IAccountSummariesExtractor _accountSummariesExtractor;
        private readonly IWebDriver _webDriver;

        public PositionsPage(IWebDriver webDriver,
            IAccountSummariesExtractor accountSummariesExtractor,
            IAccountDetailsExtractor accountDetailsExtractor)
        {
            _webDriver = webDriver;
            _accountSummariesExtractor = accountSummariesExtractor;
            _accountDetailsExtractor = accountDetailsExtractor;
        }

        public IEnumerable<IAccountSummary> GetAccountSummaries()
        {
            return _accountSummariesExtractor.ExtractAccountSummaries(_webDriver);
        }

        public IEnumerable<IAccountDetails> GetAccountDetails()
        {
            _webDriver.FindElement(By.ClassName("account-selector--tab-all")).Click();

            return _accountDetailsExtractor.ExtractAccountDetails(_webDriver);
        }
    }
}