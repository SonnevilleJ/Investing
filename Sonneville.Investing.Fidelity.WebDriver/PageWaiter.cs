using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Sonneville.Investing.Fidelity.WebDriver
{
    public interface IPageWaiter
    {
        void WaitUntilNotDisplayed(IWebDriver webDriver, By selector);
    }

    public class PageWaiter : IPageWaiter
    {
        public void WaitUntilNotDisplayed(IWebDriver webDriver, By selector)
        {
            new WebDriverWait(webDriver, TimeSpan.FromMinutes(1))
                .Until(driver => !driver.FindElement(selector).Displayed);
        }
    }
}
