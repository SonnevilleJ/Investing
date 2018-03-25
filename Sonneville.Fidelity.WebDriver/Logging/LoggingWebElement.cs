using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Sonneville.Fidelity.WebDriver.Logging
{
    public class LoggingWebElement : WebElementBase
    {
        private readonly ILog _log;

        public LoggingWebElement(IWebElement webElement, ILog log)
            : base(webElement)
        {
            _log = log ?? LogManager.GetLogger(typeof(LoggingWebElement));
        }

        public override IWebElement FindElement(By by)
        {
            _log.Trace($"Finding element {by}.");
            return Wrap(base.FindElement(@by));
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            _log.Trace($"Finding elements {by}.");
            return base.FindElements(by)
                .Select(Wrap)
                .ToList()
                .AsReadOnly();
        }

        public override void Clear()
        {
            _log.Trace($"Clearing tag `{base.TagName}`");
            base.Clear();
        }

        public override void SendKeys(string text)
        {
            _log.Verbose($"Sending keys: `{text}` to tag `{base.TagName}`.");
            base.SendKeys(text);
        }

        public override void Submit()
        {
            _log.Trace($"Submitting tag `{base.TagName}` with text `{base.Text}`");
            base.Submit();
        }

        public override void Click()
        {
            _log.Trace($"Clicking tag `{base.TagName}` with text `{base.Text}`");
            base.Click();
        }

        public override string GetAttribute(string attributeName)
        {
            var attribute = base.GetAttribute(attributeName);
            _log.Trace($"Got attribute `{attributeName}` for tag `{base.TagName}`: `{attribute}`");
            return attribute;
        }

        public override string GetProperty(string propertyName)
        {
            var property = base.GetProperty(propertyName);
            _log.Trace($"Got property `{propertyName}` for tag `{base.TagName}: `{property}`");
            return property;
        }

        public override string GetCssValue(string propertyName)
        {
            var cssValue = base.GetCssValue(propertyName);
            _log.Trace($"Got CSS value `{propertyName}` for tag `{base.TagName}`: `{cssValue}`");
            return cssValue;
        }

        private IWebElement Wrap(IWebElement element)
        {
            return new LoggingWebElement(element, _log);
        }
    }
}
