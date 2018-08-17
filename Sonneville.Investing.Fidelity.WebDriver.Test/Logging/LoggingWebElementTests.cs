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
        public void FindElementShouldLogThenWrap()
        {
            AssertFindElementReturnsLoggingWebElements(_outerWebElement, _innerWebElementMock, Level.Trace);
        }

        [Test]
        public void FindElementsShouldLogThenWrap()
        {
            AssertFindElementsReturnsLoggingWebElements(_outerWebElement, _innerWebElementMock, Level.Trace);
        }

        [Test]
        public void ShouldLogThenClear()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                subject => subject.Clear(), Level.Trace);
        }

        [Test]
        public void ShouldLogThenClick()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                webElement => webElement.Click(), Level.Trace);
        }

        [Test]
        public void ShouldLogThenGetAttribute()
        {
            var input = "input";
            AssertSubjectLogsAfterInvokingDependency(_outerWebElement, _innerWebElementMock,
                webElement => webElement.GetAttribute(input), input, Level.Trace);
        }

        [Test]
        public void ShouldLogThenGetCssValue()
        {
            var input = "input";
            AssertSubjectLogsAfterInvokingDependency(_outerWebElement, _innerWebElementMock,
                webElement => webElement.GetCssValue(input), input, Level.Trace);
        }

        [Test]
        public void ShouldLogThenGetProperty()
        {
            var input = "input";
            AssertSubjectLogsAfterInvokingDependency(_outerWebElement, _innerWebElementMock,
                webElement => webElement.GetProperty(input), input, Level.Trace);
        }

        [Test]
        public void ShouldLogThenSendKeys()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                webElement => webElement.SendKeys("hi"), Level.Verbose);
        }

        [Test]
        public void ShouldLogThenSubmit()
        {
            AssertSubjectWaitsBeforeInvokingDependency(_outerWebElement, _innerWebElementMock,
                webElement => webElement.Submit(), Level.Trace);
        }

        private void AssertFindElementReturnsLoggingWebElements<T>(
            ISearchContext searchContext,
            Mock<T> wrappedSearchContextMock,
            Level expectedLevel
        ) where T : class, ISearchContext
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            wrappedSearchContextMock.Setup(webElement => webElement.FindElement(locator))
                .Returns(innerMock.Object);

            var outer = searchContext.FindElement(locator);

            AssertSubjectWaitsBeforeInvokingDependency(outer, innerMock,
                webElement => webElement.Click(), expectedLevel,
                2
            );
        }

        private void AssertFindElementsReturnsLoggingWebElements<T>(
            ISearchContext searchContext,
            Mock<T> wrappedSearchContextMock,
            Level expectedLevel
        ) where T : class, ISearchContext
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            wrappedSearchContextMock.Setup(webElement => webElement.FindElements(locator))
                .Returns(new List<IWebElement> {innerMock.Object}.AsReadOnly());

            var outer = searchContext.FindElements(locator).Single();

            AssertSubjectWaitsBeforeInvokingDependency(outer, innerMock,
                webElement => webElement.Click(), expectedLevel, 2);
        }

        private void AssertSubjectLogsAfterInvokingDependency(
            IWebElement outer,
            Mock<IWebElement> inner,
            Expression<Func<IWebElement, string>> expectedInteraction,
            string input,
            Level expectedLevel,
            int callCount = 1)
        {
            var validationsCompleted = false;
            var returnValue = "return";
            inner.Setup(expectedInteraction)
                .Returns(returnValue);
            _loggerMock.Setup(log =>
                    log.Log(It.IsAny<Type>(), It.IsAny<Level>(), It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback<Type, Level, object, Exception>(
                    (declaringType, level, message, exception) =>
                    {
                        Assert.AreEqual(expectedLevel, level);
                        Assert.IsTrue(message.ToString().Contains(input),
                            "Did not log argument!");
                        Assert.IsTrue(message.ToString().Contains(returnValue),
                            "Did not log return value!");
                        Assert.IsNull(exception,
                            $"Exception was unexpected in log event: {exception}");
                        validationsCompleted = true;
                    }
                );
            using (var task = Task.Run(() => expectedInteraction.Compile().Invoke(outer)))
            {
                task.Wait(1000);
                Assert.IsTrue(task.IsCompleted);
                Assert.IsTrue(validationsCompleted);
                inner.Verify(expectedInteraction, Times.Once());
                _loggerMock.Verify(log =>
                    log.Log(It.IsAny<Type>(), It.IsAny<Level>(), It.IsAny<object>(), It.IsAny<Exception>()),
                    Times.Exactly(callCount), "Did not invoke logger!");
            }
        }

        private void AssertSubjectWaitsBeforeInvokingDependency(
            IWebElement outer,
            Mock<IWebElement> inner,
            Expression<Action<IWebElement>> expectedInteraction,
            Level expectedLevel,
            int callCount = 1)
        {
            var validationsCompleted = false;
            _loggerMock.Setup(log =>
                    log.Log(It.IsAny<Type>(), It.IsAny<Level>(), It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback<Type, Level, object, Exception>(
                    (declaringType, level, message, exception) =>
                    {
                        Assert.AreEqual(expectedLevel, level);
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
                    "Did not invoke logger!"));
            using (var task = Task.Run(() => expectedInteraction.Compile().Invoke(outer)))
            {
                task.Wait(1000);
                Assert.IsTrue(task.IsCompleted);
                Assert.IsTrue(validationsCompleted);
                inner.Verify(expectedInteraction, Times.Once());
            }
        }
    }
}