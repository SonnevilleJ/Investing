using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using log4net;
using OpenQA.Selenium;
using Sonneville.Utilities.Sleepers;

namespace Sonneville.Fidelity.Shell.Logging
{
    public class LoggingWebElement : IWebElement
    {
        private readonly IWebElement _webElement;
        private readonly ISleepUtil _sleepUtil;
        private readonly ILog _log;

        public LoggingWebElement(IWebElement webElement, ISleepUtil sleepUtil = null, ILog log = null)
        {
            _webElement = webElement;
            _sleepUtil = sleepUtil ?? new SleepUtil();
            _log = log ?? LogManager.GetLogger(typeof(LoggingWebElement));
        }

        public IWebElement FindElement(By by)
        {
            _log.Trace($"Finding element {by}.");
            var element = _webElement.FindElement(by);
            return new LoggingWebElement(element);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            _log.Trace($"Finding elements {by}.");
            var elements = _webElement.FindElements(by);
            return elements.Select(e => new LoggingWebElement(e) as IWebElement).ToList().AsReadOnly();
        }

        public void Clear()
        {
            _log.Trace($"Clearing tag `{_webElement.TagName}`");
            _webElement.Clear();
        }

        public void SendKeys(string text)
        {
            _log.Verbose($"Sending keys: `{text}` to tag `{_webElement.TagName}`.");
            _webElement.SendKeys(text);
        }

        public void Submit()
        {
            _log.Trace($"Submitting tag `{_webElement.TagName}` with text `{_webElement.Text}`");
            _webElement.Submit();
        }

        public void Click()
        {
            _log.Trace($"Clicking tag `{_webElement.TagName}` with text `{_webElement.Text}`");
            _webElement.Click();
            _sleepUtil.Sleep(1000);
        }

        public string GetAttribute(string attributeName)
        {
            var attribute = _webElement.GetAttribute(attributeName);
            _log.Trace($"Got attribute `{attributeName}` for tag `{_webElement.TagName}`: `{attribute}`");
            return attribute;
        }

        public string GetProperty(string propertyName)
        {
            var property = _webElement.GetProperty(propertyName);
            _log.Trace($"Got property `{propertyName}` for tag `{_webElement.TagName}: `{property}`");
            return property;
        }

        public string GetCssValue(string propertyName)
        {
            var cssValue = _webElement.GetCssValue(propertyName);
            _log.Trace($"Got CSS value `{propertyName}` for tag `{_webElement.TagName}`: `{cssValue}`");
            return cssValue;
        }

        public string TagName => _webElement.TagName;

        public string Text => _webElement.Text;

        public bool Enabled => _webElement.Enabled;

        public bool Selected => _webElement.Selected;

        public Point Location => _webElement.Location;

        public Size Size => _webElement.Size;

        public bool Displayed => _webElement.Displayed;
    }
}
