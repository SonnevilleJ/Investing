using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Sonneville.Fidelity.Shell.Logging
{
    public class LoggingWebDriver : WebDriverBase
    {
        private readonly ILog _log;

        public LoggingWebDriver(IWebDriver webDriver, ILog log) : base(webDriver)
        {
            _log = log;
        }

        public override IWebElement FindElement(By by)
        {
            _log.Trace($"Finding element {by}.");
            return base.FindElement(by);
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            _log.Trace($"Finding elements {by}.");
            var elements = base.FindElements(by);
            return elements.Select(e => new LoggingWebElement(e) as IWebElement).ToList().AsReadOnly();
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
    }
}
