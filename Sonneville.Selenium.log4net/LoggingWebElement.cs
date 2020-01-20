using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Sonneville.Selenium.log4net
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
            LogExtentions.Trace(_log, $"Finding element {by}.");
            return Wrap(base.FindElement(by));
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            LogExtentions.Trace(_log, $"Finding elements {by}.");
            return base.FindElements(by)
                .Select(Wrap)
                .ToList()
                .AsReadOnly();
        }

        public override void Clear()
        {
            LogExtentions.Trace(_log, $"Clearing tag `{base.TagName}`");
            base.Clear();
        }

        public override void SendKeys(string text)
        {
            LogExtentions.Verbose(_log, $"Sending keys: `{text}` to tag `{base.TagName}`.");
            base.SendKeys(text);
        }

        public override void Submit()
        {
            LogExtentions.Trace(_log, $"Submitting tag `{base.TagName}` with text `{base.Text}`");
            base.Submit();
        }

        public override void Click()
        {
            LogExtentions.Trace(_log, $"Clicking tag `{base.TagName}` with text `{base.Text}`");
            base.Click();
        }

        public override string GetAttribute(string attributeName)
        {
            var attribute = base.GetAttribute(attributeName);
            LogExtentions.Trace(_log, $"Got attribute `{attributeName}` for tag `{base.TagName}`: `{attribute}`");
            return attribute;
        }

        public override string GetProperty(string propertyName)
        {
            var property = base.GetProperty(propertyName);
            LogExtentions.Trace(_log, $"Got property `{propertyName}` for tag `{base.TagName}: `{property}`");
            return property;
        }

        public override string GetCssValue(string propertyName)
        {
            var cssValue = base.GetCssValue(propertyName);
            LogExtentions.Trace(_log, $"Got CSS value `{propertyName}` for tag `{base.TagName}`: `{cssValue}`");
            return cssValue;
        }

        private IWebElement Wrap(IWebElement element)
        {
            return new LoggingWebElement(element, _log);
        }
    }
}