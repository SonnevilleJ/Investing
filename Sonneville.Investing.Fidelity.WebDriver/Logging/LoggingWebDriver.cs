using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Sonneville.Investing.Fidelity.WebDriver.Logging
{
    public class LoggingWebDriver : WebDriverBase
    {
        private readonly ILog _webDriverLog;
        private readonly ILog _webElementLog;

        public LoggingWebDriver(IWebDriver webDriver, ILog webDriverLog, ILog webElementLog) : base(webDriver)
        {
            _webDriverLog = webDriverLog;
            _webElementLog = webElementLog;
        }

        public override string Url
        {
            get => base.Url;
            set
            {
                _webDriverLog.Trace($"Setting web driver URL: {value}");
                base.Url = value;
            }
        }

        public override IWebElement FindElement(By by)
        {
            _webDriverLog.Trace($"Finding element {by}.");
            return Wrap(base.FindElement(by));
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            _webDriverLog.Trace($"Finding elements {by}.");
            return base.FindElements(by)
                .Select(Wrap)
                .ToList()
                .AsReadOnly();
        }

        public override void Close()
        {
            _webDriverLog.Trace("Closing web driver...");
            base.Close();
        }

        public override void Quit()
        {
            _webDriverLog.Trace("Quitting web driver...");
            base.Quit();
        }

        private IWebElement Wrap(IWebElement foundElement)
        {
            return new LoggingWebElement(foundElement, _webElementLog);
        }
    }
}