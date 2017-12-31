using System.Collections.Generic;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.Utilities.Sleepers;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface IPositionsPage : IPage
    {
        IEnumerable<IAccountSummary> GetAccountSummaries();

        IEnumerable<IAccountDetails> GetAccountDetails();
    }

    public class PositionsPage : IPositionsPage
    {
        private readonly IWebDriver _webDriver;
        private readonly IAccountSummariesExtractor _accountSummariesExtractor;
        private readonly IAccountDetailsExtractor _accountDetailsExtractor;
        private readonly ISleepUtil _sleepUtil;

        public PositionsPage(IWebDriver webDriver,
            IAccountSummariesExtractor accountSummariesExtractor,
            IAccountDetailsExtractor accountDetailsExtractor,
            ISleepUtil sleepUtil)
        {
            _webDriver = webDriver;
            _accountSummariesExtractor = accountSummariesExtractor;
            _accountDetailsExtractor = accountDetailsExtractor;
            _sleepUtil = sleepUtil;
        }

        public IEnumerable<IAccountSummary> GetAccountSummaries()
        {
            return _accountSummariesExtractor.ExtractAccountSummaries(_webDriver);
        }

        public IEnumerable<IAccountDetails> GetAccountDetails()
        {
            _sleepUtil.Sleep(1000);
            return _accountDetailsExtractor.ExtractAccountDetails(_webDriver);
        }
    }
}
