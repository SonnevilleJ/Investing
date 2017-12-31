using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Summary;

namespace Sonneville.FidelityWebDriver.Tests.Summary
{
    [TestFixture]
    public class SummaryPageTests
    {
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

            _progressBarDivMock = new Mock<IWebElement>();

            _activityLiMock = new Mock<IWebElement>();

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

            _summaryPage = new SummaryPage(_webDriverMock.Object);
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

        [Test]
        public void ShouldNavigateToPositionsPage()
        {
            SetupVisibleProgressBar();
            _positionsLiMock.Setup(li => li.Click()).Callback(() =>
            {
                AssertInvisibleProgressBar();
                SetupVisibleProgressBar();
            });

            _summaryPage.GoToPositionsPage();

            _positionsLiMock.Verify(li => li.Click());
            AssertInvisibleProgressBar();
        }

        [Test]
        public void ShouldNavigateToActivityPage()
        {
            SetupVisibleProgressBar();
            _activityLiMock.Setup(li => li.Click()).Callback(() =>
            {
                AssertInvisibleProgressBar();
                SetupVisibleProgressBar();
            });

            _summaryPage.GoToActivityPage();

            _activityLiMock.Verify(li => li.Click());
            AssertInvisibleProgressBar();
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
