using System;
using System.Globalization;
using System.Threading;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Sonneville.Investing.Fidelity.Utilities;

namespace Sonneville.Investing.Fidelity.WebDriver.Summary
{
    public interface ISummaryPage : IPage
    {
        double GetBalanceOfAllAccounts();

        double GetGainLossAmount();

        double GetGainLossPercent();

        void GoToPositionsPage();

        void GoToActivityPage();
    }

    public class SummaryPage : ISummaryPage
    {
        private readonly IWebDriver _webDriver;
        private readonly ILog _log;

        public SummaryPage(IWebDriver webDriver, ILog log)
        {
            _webDriver = webDriver;
            _log = log;
        }

        public double GetBalanceOfAllAccounts()
        {
            var balanceText = _webDriver.FindElement(By.ClassName("js-total-balance-value")).Text;

            return NumberParser.ParseDouble(balanceText, NumberStyles.Currency);
        }

        public double GetGainLossAmount()
        {
            var text = _webDriver.FindElement(By.ClassName("js-today-change-value-dollar")).Text;

            return NumberParser.ParseDouble(text, NumberStyles.Currency);
        }

        public double GetGainLossPercent()
        {
            var text = _webDriver.FindElement(By.ClassName("js-today-change-value-percent")).Text;

            var tempt = text.Replace("(", "").Replace(")", "").Replace(" ", "");
            var result = tempt.Replace(CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");

            return NumberParser.ParseDouble(result) / 100.0;
        }

        public void GoToPositionsPage()
        {
            WaitUntilNotDisplayed(_webDriver, By.ClassName("progress-bar"));
            ActivateTab(By.CssSelector("[data-tab-name='Positions']"));
            WaitUntilNotDisplayed(_webDriver, By.ClassName("progress-bar"));
        }

        public void GoToActivityPage()
        {
            WaitUntilNotDisplayed(_webDriver, By.ClassName("progress-bar"));
            ActivateTab(By.CssSelector("[data-tab-name='Activity']"));
            WaitUntilNotDisplayed(_webDriver, By.ClassName("progress-bar"));
        }

        private void ActivateTab(By by)
        {
            do
            {
                _webDriver.FindElement(by).Click();
            } while (!TabIsSelected(by));
        }

        private bool TabIsSelected(By by)
        {
            var webElement = _webDriver.FindElement(by);
            var attribute = webElement.GetAttribute("class");
            var tabIsSelected = attribute.Contains("tabs--selected");
            return tabIsSelected;
        }

        private void WaitUntilNotDisplayed(IWebDriver webDriver, By element)
        {
            new WebDriverWait(webDriver, TimeSpan.FromMinutes(1))
                .Until(driver => !driver.FindElement(element).Displayed);
        }
    }
}
