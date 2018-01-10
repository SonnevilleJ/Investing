using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace Sonneville.Fidelity.Shell.Logging
{
    public class PatientWebElement : WebElementBase
    {
        private readonly ISeleniumWaiter _seleniumWaiter;
        private readonly TimeSpan _timespan;

        public PatientWebElement(ISeleniumWaiter seleniumWaiter,
            IWebElement webElement,
            TimeSpan timespan
        )
            : base(webElement)
        {
            _seleniumWaiter = seleniumWaiter;
            _timespan = timespan;
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
            return new PatientWebElement(_seleniumWaiter, foundElement, _timespan);
        }

        private void WaitUntilDisplayed()
        {
            _seleniumWaiter.WaitUntil(_ => Displayed, _timespan);
        }
    }
}