using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using OpenQA.Selenium;

namespace Sonneville.Fidelity.Shell.Logging
{
    public class PatientWebElement : IWebElement
    {
        private readonly ISeleniumWaiter _seleniumWaiter;
        private readonly IWebElement _webElement;
        private readonly TimeSpan _timespan;

        public PatientWebElement(ISeleniumWaiter seleniumWaiter,
            IWebElement webElement,
            TimeSpan timespan
        )
        {
            _seleniumWaiter = seleniumWaiter;
            _webElement = webElement;
            _timespan = timespan;
        }

        public IWebElement FindElement(By by)
        {
            return WrapChild(_webElement.FindElement(by));
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _webElement.FindElements(by)
                .Select(WrapChild)
                .ToList()
                .AsReadOnly();
        }

        public void Clear()
        {
            WaitUntilDisplayed();
            _webElement.Clear();
        }

        public void SendKeys(string text)
        {
            WaitUntilDisplayed();
            _webElement.SendKeys(text);
        }

        public void Submit()
        {
            WaitUntilDisplayed();
            _webElement.Submit();
        }

        public void Click()
        {
            WaitUntilDisplayed();
            _webElement.Click();
        }

        public string GetAttribute(string attributeName)
        {
            return _webElement.GetAttribute(attributeName);
        }

        public string GetProperty(string propertyName)
        {
            return _webElement.GetProperty(propertyName);
        }

        public string GetCssValue(string propertyName)
        {
            return _webElement.GetCssValue(propertyName);
        }

        public string TagName => _webElement.TagName;

        public string Text => _webElement.Text;

        public bool Enabled => _webElement.Enabled;

        public bool Selected => _webElement.Selected;

        public Point Location => _webElement.Location;

        public Size Size => _webElement.Size;

        public bool Displayed => _webElement.Displayed;

        private IWebElement WrapChild(IWebElement foundElement)
        {
            return new PatientWebElement(_seleniumWaiter, foundElement, _timespan);
        }

        private void WaitUntilDisplayed()
        {
            _seleniumWaiter.WaitUntil(_ => _webElement.Displayed, _timespan);
        }
    }
}
