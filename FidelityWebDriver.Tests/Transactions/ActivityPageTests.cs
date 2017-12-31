using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Transactions;

namespace Sonneville.FidelityWebDriver.Tests.Transactions
{
    [TestFixture]
    public class ActivityPageTests
    {
        private DateTime _expectedStartDate;
        private DateTime _expectedEndDate;
        private List<IFidelityTransaction> _expectedTransactions;

        private Mock<IWebElement> _historyRootDivMock;
        private Mock<IWebElement> _historyExpanderLinkMock;
        private Mock<IWebElement> _historyRangeDropdownMock;
        private Mock<IWebElement> _customHistoryRangeOptionMock;
        private Mock<IWebElement> _setTimePeriodButtonMock;
        private Mock<IWebElement> _dateRangeDivMock;
        private Mock<IWebElement> _fromDateInputMock;
        private Mock<IWebElement> _toDateInputMock;
        private Mock<IWebElement> _progressBarDivMock;

        private Mock<IWebDriver> _webDriverMock;
        private Mock<IHistoryTransactionParser> _historyTransactionParserMock;

        private ActivityPage _activityPage;

        [SetUp]
        public void Setup()
        {
            _expectedStartDate = DateTime.Today.AddDays(-30);
            _expectedEndDate = DateTime.Today;

            _webDriverMock = new Mock<IWebDriver>();

            _historyRootDivMock = new Mock<IWebElement>();
            _historyRootDivMock.Setup(div => div.GetAttribute("class")).Returns("activity--expander activity--expander-history expander last collapsed");

            _webDriverMock.Setup(driver => driver.FindElement(By.ClassName("activity--expander-history")))
                .Returns(_historyRootDivMock.Object);

            _customHistoryRangeOptionMock = new Mock<IWebElement>();
            _customHistoryRangeOptionMock.Setup(option => option.Click())
                .Callback(() => _customHistoryRangeOptionMock.Setup(option => option.Selected).Returns(true));

            _historyRangeDropdownMock = new Mock<IWebElement>();
            _historyRangeDropdownMock.Setup(dropdown => dropdown.FindElement(By.CssSelector("option[value='custom']")))
                .Returns(_customHistoryRangeOptionMock.Object);
            _historyRootDivMock.Setup(div => div.FindElement(By.Id("activity--history-range-dropdown")))
                .Returns(_historyRangeDropdownMock.Object);

            _setTimePeriodButtonMock = new Mock<IWebElement>();

            _historyExpanderLinkMock = new Mock<IWebElement>();
            _historyRootDivMock.Setup(div => div.FindElement(By.Id("historyExpander")))
                .Returns(_historyExpanderLinkMock.Object);

            _fromDateInputMock = new Mock<IWebElement>();

            _toDateInputMock = new Mock<IWebElement>();

            _dateRangeDivMock = new Mock<IWebElement>();
            _dateRangeDivMock.Setup(div => div.FindElement(By.ClassName("activity--history-custom-date-from-field")))
                .Returns(_fromDateInputMock.Object);
            _dateRangeDivMock.Setup(div => div.FindElement(By.ClassName("activity--history-custom-date-to-field")))
                .Returns(_toDateInputMock.Object);
            _dateRangeDivMock.Setup(
                    div => div.FindElement(By.ClassName("activity--history-custom-date-display-button")))
                .Returns(_setTimePeriodButtonMock.Object);
            _historyRootDivMock.Setup(
                    div => div.FindElement(By.ClassName("activity--history-custom-date-container")))
                .Returns(_dateRangeDivMock.Object);

            _progressBarDivMock = new Mock<IWebElement>();

            _webDriverMock.Setup(webDriver => webDriver.FindElement(By.ClassName("progress-bar")))
                .Returns(_progressBarDivMock.Object);

            _expectedTransactions = new List<IFidelityTransaction>
            {
                new FidelityTransaction()
            };

            _historyTransactionParserMock = new Mock<IHistoryTransactionParser>();
            _historyTransactionParserMock.Setup(parser => parser.ParseFidelityTransactions(_historyRootDivMock.Object))
                .Returns(_expectedTransactions);

            _activityPage = new ActivityPage(_webDriverMock.Object,
                _historyTransactionParserMock.Object);
        }

        [Test]
        public void GetHistoryShouldWaitAfterExpandingHistory()
        {
            SetupVisibleProgressBar();
            _historyExpanderLinkMock.Setup(link => link.Click())
                .Callback(AssertInvisibleProgressBar);

            _activityPage.GetTransactions(_expectedStartDate, _expectedEndDate);

            _historyExpanderLinkMock.Verify(link => link.Click());
        }

        [Test]
        public void ShouldNotExpandHistoryIfAlreadyExpanded()
        {
            _historyRootDivMock.Setup(div => div.GetAttribute("class")).Returns("activity--expander activity--expander-history expander last expanded");

            _activityPage.GetTransactions(_expectedStartDate, _expectedEndDate);

            _historyExpanderLinkMock.Verify(link => link.Click(), Times.Never);
        }

        [Test]
        public void GetHistoryShouldSetTimePeriod()
        {
            _historyExpanderLinkMock.Setup(link => link.Click())
                .Callback(SetupVisibleProgressBar);
            _setTimePeriodButtonMock.Setup(button => button.Click())
                .Callback(() =>
                {
                    _fromDateInputMock.Verify(input => input.SendKeys(_expectedStartDate.ToString("MM/dd/yyyy")));
                    _toDateInputMock.Verify(input => input.SendKeys(_expectedEndDate.ToString("MM/dd/yyyy")));
                    Assert.IsTrue(_customHistoryRangeOptionMock.Object.Selected);
                    AssertInvisibleProgressBar();
                });

            _activityPage.GetTransactions(_expectedStartDate, _expectedEndDate);

            _setTimePeriodButtonMock.Verify(button => button.Click());
        }

        [Test]
        public void GetHistoryShouldWaitAfterSettingTimePeriod()
        {
            _setTimePeriodButtonMock.Setup(button => button.Click())
                .Callback(SetupVisibleProgressBar);
            _historyTransactionParserMock.Setup(parser => parser.ParseFidelityTransactions(_historyRootDivMock.Object))
                .Callback(AssertInvisibleProgressBar)
                .Returns(new List<IFidelityTransaction>());

            _activityPage.GetTransactions(_expectedStartDate, _expectedEndDate);

            _historyTransactionParserMock.Verify(
                parser => parser.ParseFidelityTransactions(_historyRootDivMock.Object));
        }

        [Test]
        public void GetHistoryShouldReturnTransactions()
        {
            var actualTransactions = _activityPage.GetTransactions(_expectedStartDate, _expectedEndDate);

            AssertInvisibleProgressBar();
            CollectionAssert.AreEquivalent(_expectedTransactions, actualTransactions);
        }

        [Test]
        [TestCase("11/12/2012", "11/11/2013")]
        public void ShouldMakeMultipleCallsInSequential90DayChunks(string startDateString, string endDateString)
        {
            _expectedStartDate = DateTime.Parse(startDateString);
            _expectedEndDate = DateTime.Parse(endDateString);
            var inputtedStartDates = new List<DateTime>();
            var inputtedEndDates = new List<DateTime>();
            _fromDateInputMock.Setup(input => input.SendKeys(It.IsAny<string>()))
                .Callback<string>(dateString => inputtedStartDates.Add(DateTime.Parse(dateString)));
            _toDateInputMock.Setup(input => input.SendKeys(It.IsAny<string>()))
                .Callback<string>(dateString => inputtedEndDates.Add(DateTime.Parse(dateString)));

            _activityPage.GetTransactions(_expectedStartDate, _expectedEndDate);

            Assert.AreEqual(inputtedStartDates.Count, inputtedEndDates.Count, "Should set start/end dates in pairs.");
            Assert.Greater(inputtedStartDates.Count, 1, "Date ranges longer than 90 days should be split into multiple 90-day calls.");
            for (var i = 0; i < inputtedStartDates.Count; i++)
            {
                var startDate = inputtedStartDates[i];
                var endDate = inputtedEndDates[i];

                Assert.LessOrEqual(startDate, endDate);
                VerifyWithinRange(startDate, _expectedStartDate, _expectedEndDate);
                VerifyWithinRange(endDate, _expectedStartDate, _expectedEndDate);
                if (i > 0)
                {
                    var previousStartDate = inputtedStartDates[i - 1];
                    var previousEndDate = inputtedEndDates[i - 1];
                    Assert.AreEqual(TimeSpan.FromDays(1), startDate - previousEndDate);
                    Assert.LessOrEqual(startDate - previousStartDate, TimeSpan.FromDays(90));
                    Assert.LessOrEqual(endDate - previousEndDate, TimeSpan.FromDays(90));
                }
            }
        }

        [Test]
        [TestCase("11/12/2013", "11/11/2013")]
        public void ShouldThrowForBackwardsDateRanges(string startDateString, string endDateString)
        {
            _expectedStartDate = DateTime.Parse(startDateString);
            _expectedEndDate = DateTime.Parse(endDateString);

            Assert.Throws<ArgumentException>(() => _activityPage.GetTransactions(_expectedStartDate, _expectedEndDate));
        }

        [Test]
        public void ShouldThrowForDatesAfterToday()
        {
            _expectedStartDate = DateTime.Today.AddDays(-1);
            _expectedEndDate = DateTime.Today.AddDays(1);

            Assert.Throws<ArgumentException>(() => _activityPage.GetTransactions(_expectedStartDate, _expectedEndDate));
        }

        private static void VerifyWithinRange(DateTime dateTime, DateTime expectedStartDate, DateTime expectedEndDate)
        {
            Assert.GreaterOrEqual(dateTime, expectedStartDate);
            Assert.LessOrEqual(dateTime, expectedEndDate);
        }

        private void SetupVisibleProgressBar()
        {
            _progressBarDivMock.Setup(div => div.Displayed)
                .Returns(() =>
                {
                    try
                    {
                        return true;
                    }
                    finally
                    {
                        _progressBarDivMock.Setup(div => div.Displayed).Returns(false);
                    }
                });
        }

        private void AssertInvisibleProgressBar()
        {
            Assert.IsFalse(_progressBarDivMock.Object.Displayed, "Progress bar div is obstructing element!");
        }
    }
}
