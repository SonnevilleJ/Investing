using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Sonneville.Selenium.Utilities.Logging
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
                LogExtentions.Trace(_webDriverLog, $"Setting web driver URL: {value}");
                base.Url = value;
            }
        }

        public override IWebElement FindElement(By by)
        {
            LogExtentions.Trace(_webDriverLog, $"Finding element {by}.");
            return Wrap(base.FindElement(by));
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            LogExtentions.Trace(_webDriverLog, $"Finding elements {by}.");
            return base.FindElements(by)
                .Select(Wrap)
                .ToList()
                .AsReadOnly();
        }

        public override void Close()
        {
            LogExtentions.Trace(_webDriverLog, "Closing web driver...");
            base.Close();
        }

        public override void Quit()
        {
            LogExtentions.Trace(_webDriverLog, "Quitting web driver...");
            base.Quit();
        }

        private IWebElement Wrap(IWebElement foundElement)
        {
            return new LoggingWebElement(foundElement, _webElementLog);
        }
    }
}