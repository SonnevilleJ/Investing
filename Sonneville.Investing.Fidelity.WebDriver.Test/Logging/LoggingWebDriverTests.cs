using System;
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
    public class LoggingWebDriverTests : WebDriverTestsBase<LoggingWebDriver>
    {
        private Mock<ILogger> _loggerMock;

        private Mock<ILog> _logMock;

        [SetUp]
        public override void Setup()
        {
            _loggerMock = new Mock<ILogger>();

            _logMock = new Mock<ILog>();
            _logMock.Setup(log => log.Logger).Returns(_loggerMock.Object);

            base.Setup();
        }

        protected override LoggingWebDriver InstantiateWebDriverWrapper(IWebDriver webDriver)
        {
            return new LoggingWebDriver(_logMock.Object, webDriver);
        }

        [Test]
        public void ShouldLogOnFindElement()
        {
            var by = By.Id("it");
            var webElementMock = new Mock<IWebElement>();
            WebDriverMock.Setup(webDriver => webDriver.FindElement(by)).Returns(webElementMock.Object);

            WebDriverWrapper.FindElement(by);

            _loggerMock.Verify(logger => logger.Log(It.IsAny<Type>(), Level.Trace, It.IsAny<object>(), null));
        }

        protected override void AssertSubjectInvokesDependencyCorrectly(
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