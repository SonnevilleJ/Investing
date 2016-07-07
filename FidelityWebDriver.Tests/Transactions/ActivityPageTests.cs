﻿using System;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Navigation;
using Sonneville.FidelityWebDriver.Tests.Navigation;
using Sonneville.FidelityWebDriver.Transactions;
using Sonneville.FidelityWebDriver.Transactions.CSV;

namespace Sonneville.FidelityWebDriver.Tests.Transactions
{
    [TestFixture]
    public class ActivityPageTests : PageFactoryTests<IActivityPage>
    {
        private ActivityPage _activityPage;

        private Mock<IWebDriver> _webDriverMock;

        private Mock<IPageFactory> _pageFactoryMock;

        private Mock<IWebElement> _downloadLinkMock;
        private Mock<ICsvDownloadService> _downloadServiceMock;
        private string _fileContents;
        private Mock<IWebElement> _historyExpanderLinkMock;
        private Mock<IWebElement> _historyRangeDropdownMock;
        private Mock<IWebElement> _customHistoryRangeOptionMock;
        private Mock<IWebElement> _setTimePeriodButtonMock;
        private Mock<IWebElement> _dateRangeDiv;
        private DateTime _startDate;
        private DateTime _endDate;
        private Mock<IWebElement> _fromDateInputMock;
        private Mock<IWebElement> _toDateInputMock;

        [SetUp]
        public void Setup()
        {
            _startDate = DateTime.MinValue;
            _endDate = DateTime.MaxValue;

            _fileContents = "file contents";

            _downloadServiceMock = new Mock<ICsvDownloadService>();
            _downloadServiceMock.Setup(service => service.GetDownloadedContent())
                .Returns(() =>
                {
                    _downloadLinkMock.Verify(link => link.Click());
                    return _fileContents;
                });

            _customHistoryRangeOptionMock = new Mock<IWebElement>();
            _customHistoryRangeOptionMock.Setup(option => option.Click())
                .Callback(() => _customHistoryRangeOptionMock.Setup(option => option.Selected).Returns(true));

            _historyRangeDropdownMock = new Mock<IWebElement>();
            _historyRangeDropdownMock.Setup(dropdown => dropdown.FindElement(By.CssSelector("option[value='custom']")))
                .Returns(_customHistoryRangeOptionMock.Object);

            _setTimePeriodButtonMock = new Mock<IWebElement>();
            _setTimePeriodButtonMock.Setup(button => button.Click())
                .Callback(() =>
                {
                    _fromDateInputMock.Verify(input => input.SendKeys(_startDate.ToString("MM/dd/yyyy")));
                    _toDateInputMock.Verify(input => input.SendKeys(_endDate.ToString("MM/dd/yyyy")));
                    Assert.IsTrue(_customHistoryRangeOptionMock.Object.Selected);
                });

            _downloadLinkMock = new Mock<IWebElement>();
            _downloadLinkMock.Setup(link => link.Click())
                .Callback(() =>
                {
                    _setTimePeriodButtonMock.Verify(button => button.Click());
                    _downloadServiceMock.Verify(service => service.Cleanup(), Times.Once());
                });

            _historyExpanderLinkMock = new Mock<IWebElement>();

            _fromDateInputMock = new Mock<IWebElement>();

            _toDateInputMock = new Mock<IWebElement>();

            _dateRangeDiv = new Mock<IWebElement>();
            _dateRangeDiv.Setup(div => div.FindElement(By.ClassName("activity--history-custom-date-from-field")))
                .Returns(_fromDateInputMock.Object);
            _dateRangeDiv.Setup(div => div.FindElement(By.ClassName("activity--history-custom-date-to-field")))
                .Returns(_toDateInputMock.Object);
            _dateRangeDiv.Setup(div => div.FindElement(By.ClassName("activity--history-custom-date-display-button")))
                .Returns(_setTimePeriodButtonMock.Object);

            _webDriverMock = new Mock<IWebDriver>();
            _webDriverMock.Setup(webDriver => webDriver.FindElement(By.Id("historyExpander")))
                .Returns(_historyExpanderLinkMock.Object);
            _webDriverMock.Setup(webDriver => webDriver.FindElement(By.ClassName("activity--history-download-link")))
                .Returns(_downloadLinkMock.Object);
            _webDriverMock.Setup(webDriver => webDriver.FindElement(By.Id("activity--history-range-dropdown")))
                .Returns(_historyRangeDropdownMock.Object);
            _webDriverMock.Setup(
                webDriver => webDriver.FindElement(By.ClassName("activity--history-custom-date-container")))
                .Returns(_dateRangeDiv.Object);

            _pageFactoryMock = new Mock<IPageFactory>();

            _activityPage = new ActivityPage(_webDriverMock.Object, _pageFactoryMock.Object, _downloadServiceMock.Object);
        }

        [Test]
        public void DownloadHistoryShouldClickHistoryExpander()
        {
            var actualContents = _activityPage.DownloadHistory(_startDate, _endDate);

            Assert.AreEqual(_fileContents, actualContents);
            _downloadServiceMock.Verify(service => service.Cleanup(), Times.Exactly(2));
        }
    }
}