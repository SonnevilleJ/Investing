using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Investing.Fidelity.WebDriver.Configuration;
using Sonneville.Investing.Fidelity.WebDriver.Logging;

namespace Sonneville.Investing.Fidelity.WebDriver.Test.Logging
{
    [TestFixture]
    public class PatientWebDriverTests : WebDriverTestsBase<PatientWebDriver>
    {
        private bool _isDisplayed;

        private Mock<ISeleniumWaiter> _seleniumWaiterMock;
        private TimeSpan _timeSpan;

        private SeleniumConfiguration _seleniumConfiguration;

        [SetUp]
        public override void Setup()
        {
            _seleniumWaiterMock = new Mock<ISeleniumWaiter>();
            _timeSpan = TimeSpan.FromMinutes(1);

            _seleniumConfiguration = new SeleniumConfiguration {WebElementDisplayTimeout = _timeSpan};
            
            base.Setup();
        }

        protected override PatientWebDriver InstantiateWebDriverWrapper(IWebDriver webDriver)
        {
            return new PatientWebDriver(
                _seleniumWaiterMock.Object,
                _seleniumConfiguration,
                WebDriverMock.Object
            );
        }

        protected override void AssertSubjectInvokesDependencyCorrectly(
            IWebElement outer,
            Mock<IWebElement> innerMock,
            Action<IWebElement> wrapperAction,
            Expression<Action<IWebElement>> expectedInnerInvocation
        )
        {
            var validationsCompleted = false;
            _seleniumWaiterMock.Setup(waiter =>
                    waiter.WaitUntil(It.IsAny<Func<IWebDriver, bool>>(), It.IsAny<TimeSpan>(), It.IsAny<IWebDriver>()))
                .Callback<Func<IWebDriver, bool>, TimeSpan, IWebDriver>(
                    (func, timeSpan, webDriver) =>
                    {
                        Assert.AreEqual(_isDisplayed = true, func(null),
                            "WaitUntil condition does not check if IWebElement is Displayed!");
                        Assert.AreEqual(_isDisplayed = false, func(null),
                            "WaitUntil condition does not check if IWebElement is Displayed!");
                        Assert.AreEqual(_timeSpan, timeSpan, "WaitUntil called with wrong timespan!");
                        Assert.AreEqual(WebDriverWrapper, webDriver);
                        validationsCompleted = true;
                    }
                );
            innerMock.SetupGet(webElement => webElement.Displayed).Returns(() => _isDisplayed);
            innerMock.Setup(expectedInnerInvocation)
                .Callback(() => _seleniumWaiterMock.Verify(waiter =>
                        waiter.WaitUntil(It.IsAny<Func<IWebDriver, bool>>(), It.IsAny<TimeSpan>(),
                            It.IsAny<IWebDriver>()), Times.Once(),
                    "Invoked wrapped IWebElement without waiting until it was displayed!"));
            var task = Task.Run(() => wrapperAction(outer));
            task.Wait(1000);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(validationsCompleted);
            innerMock.Verify(expectedInnerInvocation, Times.Once());
        }
    }
}