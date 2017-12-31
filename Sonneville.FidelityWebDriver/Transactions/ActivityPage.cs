using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Sonneville.FidelityWebDriver.Data;

namespace Sonneville.FidelityWebDriver.Transactions
{
    public interface IActivityPage : IPage
    {
        IEnumerable<IFidelityTransaction> GetTransactions(DateTime startDate, DateTime endDate);
    }

    public class ActivityPage : IActivityPage
    {
        private readonly IWebDriver _webDriver;
        private readonly IHistoryTransactionParser _historyTransactionParser;

        public ActivityPage(IWebDriver webDriver, IHistoryTransactionParser historyTransactionParser)
        {
            _webDriver = webDriver;
            _historyTransactionParser = historyTransactionParser;
        }

        public IEnumerable<IFidelityTransaction> GetTransactions(DateTime startDate, DateTime endDate)
        {
            ThrowIfDateRangeIsInvalid(startDate, endDate);

            var historyRoot = _webDriver.FindElement(By.ClassName("activity--expander-history"));

            OpenHistoryPanel(historyRoot);
            SelectCustomDateRangeOption(historyRoot);

            return SplitDateRange(startDate, endDate)
                .SelectMany(tuple => ParseTransactionsInDateRange(historyRoot, tuple.Item1, tuple.Item2))
                .ToList();
        }

        private IEnumerable<IFidelityTransaction> ParseTransactionsInDateRange(IWebElement historyRoot, DateTime startDate, DateTime endDate)
        {
            SetTimePeriod(historyRoot, startDate, endDate);

            WaitUntilNotDisplayed(_webDriver, By.ClassName("progress-bar"));

            return _historyTransactionParser.ParseFidelityTransactions(historyRoot);
        }

        private void OpenHistoryPanel(IWebElement historyRoot)
        {
            WaitUntilNotDisplayed(_webDriver, By.ClassName("progress-bar"));

            if (!historyRoot.GetAttribute("class").Contains("expanded"))
            {
                var historyExpanderLink = historyRoot.FindElement(By.Id("historyExpander"));
                historyExpanderLink.Click();
            }
        }

        private void SelectCustomDateRangeOption(IWebElement historyRoot)
        {
            var rangeDropdown = historyRoot.FindElement(By.Id("activity--history-range-dropdown"));
            rangeDropdown.FindElement(By.CssSelector("option[value='custom']")).Click();
        }

        private void SetTimePeriod(IWebElement historyRoot, DateTime startDate, DateTime endDate)
        {
            WaitUntilNotDisplayed(_webDriver, By.ClassName("progress-bar"));

            var dateRangeDiv = historyRoot.FindElement(By.ClassName("activity--history-custom-date-container"));
            var fromDateInput = dateRangeDiv.FindElement(By.ClassName("activity--history-custom-date-from-field"));
            fromDateInput.SendKeys(startDate.ToString("MM/dd/yyyy"));
            var toDateInput = dateRangeDiv.FindElement(By.ClassName("activity--history-custom-date-to-field"));
            toDateInput.SendKeys(endDate.ToString("MM/dd/yyyy"));
            var setTimePeriodButton =
                dateRangeDiv.FindElement(By.ClassName("activity--history-custom-date-display-button"));
            setTimePeriodButton.Click();
        }

        private static void WaitUntilNotDisplayed(IWebDriver webDriver, By element)
        {
            new WebDriverWait(webDriver, TimeSpan.FromMinutes(1))
                .Until(driver => !driver.FindElement(element).Displayed);
        }

        private static void ThrowIfDateRangeIsInvalid(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start Date must preceed End Date!");
            if (endDate.Date > DateTime.Today)
                throw new ArgumentException("Cannot query for future dates!");
        }

        private static IEnumerable<Tuple<DateTime, DateTime>> SplitDateRange(DateTime startDate, DateTime endDate, int dayChunkSize = 89)
        {
            DateTime chunkEnd;
            do
            {
                var startPlusChunkSize = startDate.AddDays(dayChunkSize);
                chunkEnd = startPlusChunkSize <= endDate ? startPlusChunkSize : endDate;
                yield return Tuple.Create(startDate, chunkEnd);
                startDate = chunkEnd.AddDays(1);
            } while (chunkEnd < endDate);
        }
    }
}
