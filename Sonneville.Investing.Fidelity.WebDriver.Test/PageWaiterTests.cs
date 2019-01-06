using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Sonneville.Investing.Fidelity.WebDriver.Test
{
    [TestFixture]
    public class PageWaiterTests
    {
        [SetUp]
        public void Setup()
        {
            _selector = By.ClassName("");

            _webElements = new List<Mock<IWebElement>>
            {
                SetupWebElement(),
                SetupWebElement()
            };

            _mockWebDriver = new Mock<IWebDriver>();
            _mockWebDriver.Setup(webDriver => webDriver.FindElements(_selector))
                .Returns(_webElements.Select(mock => mock.Object).ToList().AsReadOnly);

            _pageWaiter = new PageWaiter();
        }

        private PageWaiter _pageWaiter;
        private Mock<IWebDriver> _mockWebDriver;
        private By _selector;
        private List<Mock<IWebElement>> _webElements;

        private static Mock<IWebElement> SetupWebElement()
        {
            var mockWebElement = new Mock<IWebElement>();
            SetCssValue(mockWebElement, "display", "block");
            return mockWebElement;
        }

        private static void SetCssValue(Mock<IWebElement> mockWebElement, string propertyName, string value)
        {
            mockWebElement.Setup(webElement => webElement.GetCssValue(propertyName)).Returns(value);
        }

        [Test]
        public void ShouldTimeoutIfConditionNotMetForAll()
        {
            var timeout = TimeSpan.FromSeconds(1);

            var task = Task.Run(() => _pageWaiter.WaitUntilNotDisplayed(_mockWebDriver.Object, _selector, timeout));

            Assert.IsFalse(task.IsCompleted);
            var mockWebElement = _webElements.First();
            SetCssValue(mockWebElement, "display", "none");

            try
            {
                task.Wait();
                Assert.Fail("Should have thrown timeout exception but did not");
            }
            catch (AggregateException ae)
            {
                Assert.AreEqual(1, ae.InnerExceptions.Count);
                var exception = ae.InnerExceptions.Single();
                Assert.AreEqual(typeof(WebDriverTimeoutException), exception.GetType());
            }
        }

        [Test]
        public void ShouldWaitUntilConditionIsSatisfiedForAll()
        {
            var timeout = TimeSpan.FromSeconds(1);

            var task = Task.Run(() => _pageWaiter.WaitUntilNotDisplayed(_mockWebDriver.Object, _selector, timeout));

            Assert.IsFalse(task.IsCompleted);
            _webElements.ForEach(mockWebElement =>
                SetCssValue(mockWebElement, "display", "none"));
            Task.Delay(timeout).Wait();

            Assert.IsTrue(task.IsCompletedSuccessfully);
        }

        [Test]
        public void ShouldWaitUntilTimeout()
        {
            var timeout = TimeSpan.FromTicks(1);

            Assert.Throws<WebDriverTimeoutException>(() =>
                _pageWaiter.WaitUntilNotDisplayed(_mockWebDriver.Object, _selector, timeout));
        }
    }
}
