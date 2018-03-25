using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Fidelity.WebDriver.Configuration;
using Sonneville.Fidelity.WebDriver.Logging;

namespace Sonneville.Fidelity.WebDriver.Test.Logging
{
    [TestFixture]
    public class PatientWebDriverTest
    {
        private bool _isDisplayed;

        private Mock<IWebDriver> _webDriverMock;

        private Mock<ISeleniumWaiter> _seleniumWaiterMock;
        private TimeSpan _timeSpan;

        private PatientWebDriver _patientWebDriver;
        private SeleniumConfiguration _seleniumConfiguration;

        [SetUp]
        public void Setup()
        {
            _seleniumWaiterMock = new Mock<ISeleniumWaiter>();
            _timeSpan = TimeSpan.FromMinutes(1);

            _seleniumConfiguration = new SeleniumConfiguration {WebElementDisplayTimeout = _timeSpan};

            _webDriverMock = new Mock<IWebDriver>();

            _patientWebDriver = new PatientWebDriver(
                _seleniumWaiterMock.Object,
                _seleniumConfiguration,
                _webDriverMock.Object
            );
        }

        [Test]
        public void FindElementShouldCreatePatientWebElement()
        {
            AssertFindElementReturnsPatientWebElements(_patientWebDriver, _webDriverMock);
        }

        [Test]
        public void FindElementsShouldCreatePatientWebElements()
        {
            AssertFindElementsReturnsPatientWebElements(_patientWebDriver, _webDriverMock);
        }

        private void AssertFindElementReturnsPatientWebElements<T>(
            ISearchContext searchContext,
            Mock<T> webDriverMock
        ) where T : class, ISearchContext
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            webDriverMock.Setup(webDriver => webDriver.FindElement(locator))
                .Returns(innerMock.Object);

            var outer = searchContext.FindElement(locator);

            AssertSubjectWaitsBeforeInvokingDependency(outer, innerMock,
                subject => subject.Click(),
                dependency => dependency.Click()
            );
        }

        private void AssertFindElementsReturnsPatientWebElements<T>(
            ISearchContext searchContext,
            Mock<T> wrappedSearchContext
        ) where T : class, ISearchContext
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            wrappedSearchContext.Setup(webElement => webElement.FindElements(locator))
                .Returns(new List<IWebElement> {innerMock.Object}.AsReadOnly());

            var outer = searchContext.FindElements(locator).Single();

            AssertSubjectWaitsBeforeInvokingDependency(outer, innerMock,
                subject => subject.Click(),
                dependency => dependency.Click()
            );
        }

        private void AssertSubjectWaitsBeforeInvokingDependency(
            IWebElement outer,
            Mock<IWebElement> inner,
            Action<IWebElement> trigger,
            Expression<Action<IWebElement>> expectedInteraction
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
                        Assert.AreEqual(_patientWebDriver, webDriver);
                        validationsCompleted = true;
                    }
                );
            inner.SetupGet(webElement => webElement.Displayed).Returns(() => _isDisplayed);
            inner.Setup(expectedInteraction)
                .Callback(() => _seleniumWaiterMock.Verify(waiter =>
                        waiter.WaitUntil(It.IsAny<Func<IWebDriver, bool>>(), It.IsAny<TimeSpan>(),
                            It.IsAny<IWebDriver>()), Times.Once(),
                    "Invoked wrapped IWebElement without waiting until it was displayed!"));
            var task = Task.Run(() => trigger(outer));
            task.Wait(1000);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(validationsCompleted);
            inner.Verify(expectedInteraction, Times.Once());
        }
    }
}