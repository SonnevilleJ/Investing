﻿using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Sonneville.Selenium.log4net
{
    public interface ISeleniumWaiter
    {
        void WaitUntil(Func<IWebDriver, bool> condition, TimeSpan timeout, IWebDriver webDriver);
    }

    public class SeleniumWaiter : ISeleniumWaiter
    {
        public void WaitUntil(Func<IWebDriver, bool> condition, TimeSpan timeout, IWebDriver webDriver)
        {
            new WebDriverWait(webDriver, timeout)
                .Until(condition);
        }
    }
}