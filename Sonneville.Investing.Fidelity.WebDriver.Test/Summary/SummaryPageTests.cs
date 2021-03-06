﻿using System;
using log4net;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Investing.Fidelity.WebDriver.Summary;

namespace Sonneville.Investing.Fidelity.WebDriver.Test.Summary
{
    [TestFixture]
    public class SummaryPageTests
    {
        [SetUp]
        public void Setup()
        {
            _balanceNumber = 1234.56;

            _fullBalanceSpanMock = new Mock<IWebElement>();
            _fullBalanceSpanMock.Setup(span => span.Text).Returns(_balanceNumber.ToString("C"));

            _gainLossAmount = 12.35;

            _gainLossAmountSpanMock = new Mock<IWebElement>();
            _gainLossAmountSpanMock.Setup(span => span.Text).Returns(_gainLossAmount.ToString("C"));

            _gainLossPercent = 0.1;

            _gainLossPercentSpanMock = new Mock<IWebElement>();
            _gainLossPercentSpanMock.Setup(span => span.Text)
                .Returns($"(+{_gainLossPercent:P})");

            _positionsLiMock = new Mock<IWebElement>();
            _positionsLiMock.Setup(li => li.GetAttribute("class")).Returns("");

            _progressBarDivMock = new Mock<IWebElement>();

            _activityLiMock = new Mock<IWebElement>();
            _activityLiMock.Setup(li => li.GetAttribute("class")).Returns("");

            _webDriverMock = new Mock<IWebDriver>();
            _webDriverMock.Setup(driver => driver.FindElement(By.ClassName("js-total-balance-value")))
                .Returns(_fullBalanceSpanMock.Object);
            _webDriverMock.Setup(driver => driver.FindElement(By.CssSelector("[data-tab-name='Positions']")))
                .Returns(_positionsLiMock.Object);
            _webDriverMock.Setup(driver => driver.FindElement(By.CssSelector("[data-tab-name='Activity']")))
                .Returns(_activityLiMock.Object);
            _webDriverMock.Setup(driver => driver.FindElement(By.ClassName("js-today-change-value-dollar")))
                .Returns(_gainLossAmountSpanMock.Object);
            _webDriverMock.Setup(driver => driver.FindElement(By.ClassName("js-today-change-value-percent")))
                .Returns(_gainLossPercentSpanMock.Object);
            _webDriverMock.Setup(webDriver => webDriver.FindElement(By.ClassName("progress-bar")))
                .Returns(_progressBarDivMock.Object);

            _logMock = new Mock<ILog>();

            _mockPageWaiter = new Mock<IPageWaiter>();

            _summaryPage = new SummaryPage(_webDriverMock.Object, _mockPageWaiter.Object, _logMock.Object);
        }

        private SummaryPage _summaryPage;
        private Mock<IWebDriver> _webDriverMock;
        private double _balanceNumber;
        private Mock<IWebElement> _fullBalanceSpanMock;
        private Mock<IWebElement> _positionsLiMock;
        private Mock<IWebElement> _gainLossAmountSpanMock;
        private Mock<IWebElement> _gainLossPercentSpanMock;
        private double _gainLossAmount;
        private double _gainLossPercent;

        private Mock<IWebElement> _activityLiMock;
        private Mock<IWebElement> _progressBarDivMock;
        private Mock<ILog> _logMock;
        private Mock<IPageWaiter> _mockPageWaiter;

        private static void RegisterSuccessfulClick(Mock<IWebElement> liMock)
        {
            liMock.Setup(li => li.GetAttribute("class")).Returns("tabs--selected");
        }

        private void AssertInvisibleProgressBar(By selector)
        {
            _mockPageWaiter.Verify(waiter => waiter.WaitUntilNotDisplayed(_webDriverMock.Object, selector));
        }

        private void SetupVisibleProgressBar()
        {
            _mockPageWaiter.Verify(
                waiter => waiter.WaitUntilNotDisplayed(It.IsAny<IWebDriver>(), It.IsAny<By>()), Times.Never);
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

        [Test]
        public void ShouldClickActivityTabUntilSelected()
        {
            var callCount = 0;
            SetupVisibleProgressBar();
            _activityLiMock.Setup(li => li.Click()).Callback(() =>
            {
                switch (callCount++)
                {
                    case 0:
                        break;
                    case 1:
                        RegisterSuccessfulClick(_activityLiMock);
                        break;
                    default:
                        throw new InvalidOperationException("Iterated too much!");
                }
            });

            _summaryPage.GoToActivityPage();

            _activityLiMock.Verify(li => li.Click());
            AssertInvisibleProgressBar(By.ClassName("progress-bar"));
        }

        [Test]
        public void ShouldClickPositionsTabUntilSelected()
        {
            var callCount = 0;
            SetupVisibleProgressBar();
            _positionsLiMock.Setup(li => li.Click()).Callback(() =>
            {
                switch (callCount++)
                {
                    case 0:
                        break;
                    case 1:
                        RegisterSuccessfulClick(_positionsLiMock);
                        break;
                    default:
                        throw new InvalidOperationException("Iterated too much!");
                }
            });

            _summaryPage.GoToPositionsPage();

            _positionsLiMock.Verify(li => li.Click());
            AssertInvisibleProgressBar(By.ClassName("progress-bar"));
        }

        [Test]
        public void ShouldReturnBalanceOfAllAccounts()
        {
            var balance = _summaryPage.GetBalanceOfAllAccounts();

            Assert.AreEqual(_balanceNumber, balance);
        }

        [Test]
        public void ShouldReturnTodaysGainLossAmount()
        {
            var gainLossAmount = _summaryPage.GetGainLossAmount();

            Assert.AreEqual(_gainLossAmount, gainLossAmount);
        }

        [Test]
        public void ShouldReturnTodaysGainLossPercent()
        {
            var gainLossPercent = _summaryPage.GetGainLossPercent();

            Assert.AreEqual(_gainLossPercent, gainLossPercent);
        }
    }
}
