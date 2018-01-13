using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using Sonneville.Fidelity.Shell.Configuration;

namespace Sonneville.Fidelity.Shell.Logging
{
    public class PatientWebDriver : WebDriverBase
    {
        private readonly ISeleniumWaiter _seleniumWaiter;
        private readonly TimeSpan _timeSpan;

        public PatientWebDriver(ISeleniumWaiter seleniumWaiter, SeleniumConfiguration seleniumConfig, IWebDriver webDriver) :
            base(webDriver)
        {
            _seleniumWaiter = seleniumWaiter;
            _timeSpan = seleniumConfig.WebElementDisplayTimeout;
        }

        public override IWebElement FindElement(By by)
        {
            return WrapFoundElement(base.FindElement(by));
        }

        public override ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return base.FindElements(by)
                .Select(WrapFoundElement)
                .ToList()
                .AsReadOnly();
        }

        private IWebElement WrapFoundElement(IWebElement foundElement)
        {
            return new PatientWebElement(_seleniumWaiter, foundElement, _timeSpan, this);
        }
    }
}