using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Sonneville.FidelityWebDriver.Demo.log4net
{
    public class LoggingWebDriver : IWebDriver
    {
        private readonly IWebDriver _webDriver;
        private readonly ILog _log;

        public LoggingWebDriver(IWebDriver webDriver, ILog log)
        {
            _webDriver = webDriver;
            _log = log;
        }

        public IWebElement FindElement(By by)
        {
            _log.Trace($"Finding element {by}.");
            var element = _webDriver.FindElement(by);
            return new LoggingWebElement(element);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            _log.Trace($"Finding elements {by}.");
            var elements = _webDriver.FindElements(by);
            return elements.Select(e => new LoggingWebElement(e) as IWebElement).ToList().AsReadOnly();
        }

        public void Dispose()
        {
            _webDriver?.Dispose();
        }

        public void Close()
        {
            _log.Trace($"Closing web driver {_webDriver}");
            _webDriver.Close();
        }

        public void Quit()
        {
            _log.Trace($"Quitting web driver {_webDriver}");
            _webDriver.Quit();
        }

        public IOptions Manage()
        {
            return _webDriver.Manage();
        }

        public INavigation Navigate()
        {
            return _webDriver.Navigate();
        }

        public ITargetLocator SwitchTo()
        {
            return _webDriver.SwitchTo();
        }

        public string Url
        {
            get { return _webDriver.Url; }
            set
            {
                _log.Trace($"Setting web driver URL: {value}");
                _webDriver.Url = value;
            }
        }

        public string Title => _webDriver.Title;

        public string PageSource => _webDriver.PageSource;

        public string CurrentWindowHandle => _webDriver.CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles => _webDriver.WindowHandles;
    }
}
