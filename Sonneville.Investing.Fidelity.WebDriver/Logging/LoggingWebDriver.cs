using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Sonneville.Investing.Fidelity.WebDriver.Logging
{
    public class LoggingWebDriver : WebDriverBase
    {
        private readonly ILog _log;

        public LoggingWebDriver(ILog log, IWebDriver webDriver) : base(webDriver)
        {
            _log = log;
        }

        public override IWebElement FindElement(By by)
        {
            _log.Trace($"Finding element {by}.");
            return Wrap(base.FindElement(by));
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            _log.Trace($"Finding elements {by}.");
            return base.FindElements(by)
                .Select(Wrap)
                .ToList()
                .AsReadOnly();
        }

        public override void Close()
        {
            _log.Trace("Closing web driver...");
            base.Close();
        }

        public override void Quit()
        {
            _log.Trace("Quitting web driver...");
            base.Quit();
        }

        public override string Url
        {
            get => base.Url;
            set
            {
                _log.Trace($"Setting web driver URL: {value}");
                base.Url = value;
            }
        }

        private IWebElement Wrap(IWebElement foundElement)
        {
            return new LoggingWebElement(foundElement, _log);
        }
    }
}
