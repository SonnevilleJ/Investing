using System;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Fidelity.Shell.Logging;

namespace Sonneville.Fidelity.Shell.Test.Logging
{
    [TestFixture]
    public class SeleniumWaiterTests
    {
        private Mock<IWebDriver> _webDriverMock;
        private SeleniumWaiter _seleniumWaiter;
        private TimeSpan _timeout;

        [SetUp]
        public void Setup()
        {
            _webDriverMock = new Mock<IWebDriver>();

            _timeout = TimeSpan.FromMilliseconds(1000);

            _seleniumWaiter = new SeleniumWaiter(_webDriverMock.Object);
        }

        [Test]
        public void ShouldTimeoutAndThrowWhenConditionNeverMet()
        {
            var startTime = DateTime.Now;
            Assert.Throws<WebDriverTimeoutException>(() => _seleniumWaiter.WaitUntil(_ => false, _timeout));
            var endTime = DateTime.Now;

            Assert.LessOrEqual(_timeout, endTime - startTime);
        }
        
        [Test]
        public void ShouldCompleteWhenConditionMet()
        {
            var startTime = DateTime.Now;
            Assert.DoesNotThrow(() => _seleniumWaiter.WaitUntil(_ => true, _timeout));
            var endTime = DateTime.Now;

            Assert.GreaterOrEqual(_timeout, endTime - startTime);
        }
    }
}
