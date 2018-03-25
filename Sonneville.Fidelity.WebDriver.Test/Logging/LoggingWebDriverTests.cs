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
using Sonneville.Fidelity.WebDriver.Logging;

namespace Sonneville.Fidelity.WebDriver.Test.Logging
{
    [TestFixture]
    public class LoggingWebDriverTests
    {
        private Mock<ILogger> _loggerMock;

        private Mock<ILog> _logMock;

        private Mock<IWebDriver> _webDriverMock;

        private LoggingWebDriver _loggingWebDriver;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger>();

            _logMock = new Mock<ILog>();
            _logMock.Setup(log => log.Logger).Returns(_loggerMock.Object);

            _webDriverMock = new Mock<IWebDriver>();

            _loggingWebDriver = new LoggingWebDriver(_logMock.Object, _webDriverMock.Object);
        }

        [Test]
        public void ShouldLogOnFindElement()
        {
            var by = By.Id("it");
            var webElementMock = new Mock<IWebElement>();
            _webDriverMock.Setup(webDriver => webDriver.FindElement(by)).Returns(webElementMock.Object);

            _loggingWebDriver.FindElement(by);

            _loggerMock.Verify(logger => logger.Log(It.IsAny<Type>(), Level.Trace, It.IsAny<object>(), null));
        }

        [Test]
        public void FindElementShouldCreateWrappedWebElement()
        {
            AssertFindElementReturnsWrappedWebElements(_loggingWebDriver, _webDriverMock);
        }

        [Test]
        public void FindElementsShouldCreatePatientWebElements()
        {
            AssertFindElementsReturnsPatientWebElements(_loggingWebDriver, _webDriverMock);
        }

        private void AssertFindElementReturnsWrappedWebElements<T>(
            ISearchContext wrapperSearchContext,
            Mock<T> webDriverMock
        ) where T : class, ISearchContext
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            webDriverMock.Setup(webDriver => webDriver.FindElement(locator))
                .Returns(innerMock.Object);

            var outer = wrapperSearchContext.FindElement(locator);

            AssertSubjectWaitsBeforeInvokingDependency(outer, innerMock,
                wrapper => wrapper.Click(),
                inner => inner.Click()
            );
        }

        private void AssertFindElementsReturnsPatientWebElements<T>(
            ISearchContext wrapperSearchContext,
            Mock<T> innerSearchContextMock
        ) where T : class, ISearchContext
        {
            var innerMock = new Mock<IWebElement>();

            var locator = By.Id("some ID");
            innerSearchContextMock.Setup(webElement => webElement.FindElements(locator))
                .Returns(new List<IWebElement> {innerMock.Object}.AsReadOnly());

            var outer = wrapperSearchContext.FindElements(locator).Single();

            AssertSubjectWaitsBeforeInvokingDependency(outer, innerMock,
                wrapper => wrapper.Click(),
                inner => inner.Click()
            );
        }

        private void AssertSubjectWaitsBeforeInvokingDependency(
            IWebElement wrapper,
            Mock<IWebElement> innerMock,
            Action<IWebElement> wrapperAction,
            Expression<Action<IWebElement>> expectedInnerInvocation
        )
        {
            var validationsCompleted = false;
            _loggerMock.Setup(logger =>
                    logger.Log(It.IsAny<Type>(), It.IsAny<Level>(), It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback<Type, Level, object, Exception>(
                    (declaringType, level, message, exception) =>
                    {
                        Assert.AreEqual(Level.Trace, level);
                        Assert.IsNull(exception);
                        validationsCompleted = true;
                    }
                );
            innerMock.Setup(expectedInnerInvocation)
                .Callback(VerifyLoggingOccurred);
            using (var task = Task.Run(() => wrapperAction(wrapper)))
            {
                task.Wait(1000);
                innerMock.Verify(expectedInnerInvocation, Times.Once());
                Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
                Assert.IsTrue(validationsCompleted);
            }
        }

        private void VerifyLoggingOccurred()
        {
            _loggerMock.Verify(logger =>
                    logger.Log(It.IsAny<Type>(), It.IsAny<Level>(), It.IsAny<object>(), It.IsAny<Exception>()),
                Times.AtLeastOnce(), "Failed to log a trace message!");
        }
    }
}
