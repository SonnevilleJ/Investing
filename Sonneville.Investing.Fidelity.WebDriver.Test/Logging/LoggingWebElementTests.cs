using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Investing.Fidelity.WebDriver.Logging;

namespace Sonneville.Investing.Fidelity.WebDriver.Test.Logging
{
    [TestFixture]
    public class LoggingWebElementTests
    {
        private Mock<IWebElement> _innerWebElementMock;

        private Mock<ILogger> _loggerMock;
        private Mock<ILog> _logMock;

        private LoggingWebElement _outerWebElement;

        [SetUp]
        public void Setup()
        {
            _innerWebElementMock = new Mock<IWebElement>();
            _innerWebElementMock.Setup(webElement => webElement.TagName).Returns("inner");

            _loggerMock = new Mock<ILogger>();

            _logMock = new Mock<ILog>();
            _logMock.Setup(log => log.Logger).Returns(_loggerMock.Object);

            _outerWebElement = new LoggingWebElement(_innerWebElementMock.Object, _logMock.Object);
        }

        [Test]
        public void FoundElementShouldLogThenClick()
        {
            AssertFindElementReturnsLoggingWebElements(_outerWebElement, _innerWebElementMock);
        }

        [Test]
        public void FoundElementsShouldLogThenClick()
        {
            AssertFindElementsReturnsLoggingWebElements(_outerWebElement, _innerWebElementMock);
        }

        [Test]
        public void ShouldLogThenClear()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.Clear(),
                dependency => dependency.Clear()
            );
        }

        [Test]
        public void ShouldLogThenClick()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.Click(),
                dependency => dependency.Click()
            );
        }

        [Test]
        public void ShouldLogThenSubmit()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.Submit(),
                dependency => dependency.Submit()
            );
        }

        private void AssertFindElementReturnsLoggingWebElements<T>(
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
                dependency => dependency.Click(),
                2
            );
        }

        private void AssertFindElementsReturnsLoggingWebElements<T>(
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
                dependency => dependency.Click(), 2);
        }

        private void AssertSubjectWaitsBeforeInvokingDependency(
            IWebElement outer,
            Mock<IWebElement> inner,
            Action<IWebElement> trigger,
            Expression<Action<IWebElement>> expectedInteraction, int callCount = 1)
        {
            var validationsCompleted = false;
            _loggerMock.Setup(log =>
                    log.Log(It.IsAny<Type>(), It.IsAny<Level>(), It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback<Type, Level, object, Exception>(
                    (declaringType, level, message, exception) =>
                    {
                        Assert.AreEqual(Level.Trace, level);
                        Assert.IsFalse(string.IsNullOrWhiteSpace(message as string),
                            "Should have logged some helpful message!");
                        Assert.IsNull(exception,
                            $"Exception was unexpected in log event: {exception}");
                        validationsCompleted = true;
                    }
                );
            inner.Setup(expectedInteraction)
                .Callback(() => _loggerMock.Verify(log =>
                        log.Log(It.IsAny<Type>(), It.IsAny<Level>(), It.IsAny<object>(), It.IsAny<Exception>()),
                    Times.Exactly(callCount),
                    "Did not log a trace event!"));
            using (var task = Task.Run(() => trigger(outer)))
            {
                task.Wait(1000);
                Assert.IsTrue(task.IsCompleted);
                Assert.IsTrue(validationsCompleted);
                inner.Verify(expectedInteraction, Times.Once());
            }
        }
    }
}