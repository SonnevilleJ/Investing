﻿using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Sonneville.Fidelity.Shell.Logging
{
    public interface ISeleniumWaiter
    {
        void WaitUntil(Func<IWebDriver, bool> condition, TimeSpan timeout);
    }

    public class SeleniumWaiter : ISeleniumWaiter
    {
        private readonly IWebDriver _webDriver;

        public SeleniumWaiter(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public void WaitUntil(Func<IWebDriver, bool> condition, TimeSpan timeout)
        {
            new WebDriverWait(_webDriver, timeout)
                .Until(condition);
        }
    }
}