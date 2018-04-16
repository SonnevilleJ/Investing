using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace Sonneville.Investing.Fidelity.WebDriver.Logging
{
    public class PatientWebElement : WebElementBase
    {
        private readonly ISeleniumWaiter _seleniumWaiter;
        private readonly TimeSpan _timespan;
        private readonly IWebDriver _webDriver;

        public PatientWebElement(ISeleniumWaiter seleniumWaiter,
            IWebElement webElement,
            TimeSpan timespan,
            IWebDriver webDriver
        ) : base(webElement)
        {
            _seleniumWaiter = seleniumWaiter;
            _timespan = timespan;
            _webDriver = webDriver;
        }

        public override IWebElement FindElement(By by)
        {
            return WrapChild(base.FindElement(by));
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return base.FindElements(by)
                .Select(WrapChild)
                .ToList()
                .AsReadOnly();
        }

        public override void Clear()
        {
            WaitUntilDisplayed();
            base.Clear();
        }

        public override void SendKeys(string text)
        {
            WaitUntilDisplayed();
            base.SendKeys(text);
        }

        public override void Submit()
        {
            WaitUntilDisplayed();
            base.Submit();
        }

        public override void Click()
        {
            WaitUntilDisplayed();
            base.Click();
        }

        private IWebElement WrapChild(IWebElement foundElement)
        {
            return new PatientWebElement(_seleniumWaiter, foundElement, _timespan, _webDriver);
        }

        private void WaitUntilDisplayed()
        {
            _seleniumWaiter.WaitUntil(_ => Displayed, _timespan, _webDriver);
        }
    }
}