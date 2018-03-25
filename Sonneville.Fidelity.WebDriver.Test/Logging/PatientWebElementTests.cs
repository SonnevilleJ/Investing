using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Fidelity.WebDriver.Logging;

namespace Sonneville.Fidelity.WebDriver.Test.Logging
{
    [TestFixture]
    public class PatientWebElementTests
    {
        private bool _isDisplayed;
        private Mock<IWebElement> _innerWebElementMock;

        private Mock<ISeleniumWaiter> _seleniumWaiterMock;
        private TimeSpan _timeSpan;

        private PatientWebElement _outerWebElement;
        private Mock<IWebDriver> _webDriverMock;

        [SetUp]
        public void Setup()
        {
            _seleniumWaiterMock = new Mock<ISeleniumWaiter>();

            _innerWebElementMock = new Mock<IWebElement>();

            _timeSpan = TimeSpan.FromMinutes(1);

            _webDriverMock = new Mock<IWebDriver>();

            _outerWebElement = new PatientWebElement(_seleniumWaiterMock.Object,
                _innerWebElementMock.Object, _timeSpan, _webDriverMock.Object);
        }

        [Test]
        public void FoundElementShouldWaitUntilIsDisplayedThenClick()
        {
            AssertFindElementReturnsPatientWebElements(_outerWebElement, _innerWebElementMock);
        }

        [Test]
        public void FoundElementsShouldWaitUntilIsDisplayedThenClick()
        {
            AssertFindElementsReturnsPatientWebElements(_outerWebElement, _innerWebElementMock);
        }

        [Test]
        public void ShouldNotWaitToGetAttribute()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.GetAttribute("attribute"),
                dependency => dependency.GetAttribute("attribute"),
                0
            );
        }

        [Test]
        public void ShouldNotWaitToGetProperty()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.GetProperty("property"),
                dependency => dependency.GetProperty("property"),
                0
            );
        }

        [Test]
        public void ShouldNotWaitToGetCssValue()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.GetCssValue("cssvalue"),
                dependency => dependency.GetCssValue("cssvalue"),
                0
            );
        }

        [Test]
        public void ShouldWaitUntilWebElementIsDisplayedThenClear()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.Clear(),
                dependency => dependency.Clear());
        }

        [Test]
        public void ShouldWaitUntilWebElementIsDisplayedThenClick()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.Click(),
                dependency => dependency.Click());
        }

        [Test]
        public void ShouldWaitUntilWebElementIsDisplayedThenSubmit()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.Submit(),
                dependency => dependency.Submit());
        }

        [Test]
        public void ShouldWaitUntilWebElementIsDisplayedThenSendKeys()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.SendKeys("sendkeys"),
                dependency => dependency.SendKeys("sendkeys"));
        }

        private void AssertFindElementReturnsPatientWebElements<T>(
            ISearchContext searchContext,
            Mock<T> wrappedSearchContextMock
        ) where T : class, ISearchContext
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            wrappedSearchContextMock.Setup(webElement => webElement.FindElement(locator))
                .Returns(innerMock.Object);

            var outer = searchContext.FindElement(locator);

            AssertSubjectWaitsBeforeInvokingDependency(outer, innerMock,
                subject => subject.Click(),
                dependency => dependency.Click()
            );
        }

        private void AssertFindElementsReturnsPatientWebElements<T>(
            ISearchContext searchContext,
            Mock<T> wrappedSearchContextMock
        ) where T : class, ISearchContext
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            wrappedSearchContextMock.Setup(webElement => webElement.FindElements(locator))
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
            Expression<Action<IWebElement>> expectedInteraction,
            int expectedCallCount = 1
        )
        {
            var completedValidations = 0;
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
                        Assert.AreEqual(_webDriverMock.Object, webDriver);
                        completedValidations++;
                    }
                );
            inner.SetupGet(webElement => webElement.Displayed).Returns(() => _isDisplayed);
            inner.Setup(expectedInteraction)
                .Callback(() => _seleniumWaiterMock.Verify(waiter =>
                        waiter.WaitUntil(It.IsAny<Func<IWebDriver, bool>>(), It.IsAny<TimeSpan>(),
                            It.IsAny<IWebDriver>()),
                    Times.Exactly(expectedCallCount),
                    "Invoked wrapped IWebElement without waiting until it was displayed!"));
            using (var task = Task.Run(() => trigger(outer)))
            {
                task.Wait(1000);
                Assert.IsTrue(task.IsCompleted);
                Assert.AreEqual(expectedCallCount, completedValidations);
                inner.Verify(expectedInteraction, Times.Once());
            }
        }
    }
}