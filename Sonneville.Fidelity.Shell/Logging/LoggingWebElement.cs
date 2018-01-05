using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Sonneville.Fidelity.Shell.Logging
{
    public class LoggingWebElement : IWebElement
    {
        private readonly IWebElement _webElement;
        private readonly ILog _log;

        public LoggingWebElement(IWebElement webElement, ILog log)
        {
            _webElement = webElement;
            _log = log ?? LogManager.GetLogger(typeof(LoggingWebElement));
        }

        public IWebElement FindElement(By by)
        {
            _log.Trace($"Finding element {by}.");
            return Wrap(_webElement.FindElement(@by));
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            _log.Trace($"Finding elements {by}.");
            return _webElement.FindElements(by)
                .Select(Wrap)
                .ToList()
                .AsReadOnly();
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

        private IWebElement Wrap(IWebElement element)
        {
            return new LoggingWebElement(element, _log);
        }
    }
}
