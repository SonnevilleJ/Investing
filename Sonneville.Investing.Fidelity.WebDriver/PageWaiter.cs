using System;
using System.Linq;
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
            WaitUntilNotDisplayed(webDriver, selector, TimeSpan.FromMinutes(1));
        }

        public void WaitUntilNotDisplayed(IWebDriver webDriver, By selector, TimeSpan timeout)
        {
            new WebDriverWait(webDriver, timeout)
                .Until(driver => webDriver.FindElements(selector)
                    .All(element => element.GetCssValue("display") == "none"));
        }
    }
}
