using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Sonneville.Selenium.log4net.Test
{
    [TestFixture]
    public class LoggingWebDriverTests : WebDriverTestsBase<LoggingWebDriver>
    {
        [SetUp]
        public override void Setup()
        {
            _webDriverLoggerMock = new Mock<ILogger>();

            _webDriverLogMock = new Mock<ILog>();
            _webDriverLogMock.Setup(log => log.Logger).Returns(_webDriverLoggerMock.Object);

            _webElementLoggerMock = new Mock<ILogger>();

            _webElementLogMock = new Mock<ILog>();
            _webElementLogMock.Setup(log => log.Logger).Returns(_webElementLoggerMock.Object);

            base.Setup();
        }

        private Mock<ILogger> _webDriverLoggerMock;
        private Mock<ILogger> _webElementLoggerMock;

        private Mock<ILog> _webDriverLogMock;
        private Mock<ILog> _webElementLogMock;

        protected override LoggingWebDriver InstantiateWebDriverWrapper(IWebDriver webDriver)
        {
            return new LoggingWebDriver(
                webDriver,
                _webDriverLogMock.Object,
                _webElementLogMock.Object);
        }

        protected override void AssertSubjectInvokesDependencyCorrectly(
            IWebElement wrapper,
            Mock<IWebElement> innerMock,
            Action<IWebElement> wrapperAction,
            Expression<Action<IWebElement>> expectedInnerInvocation
        )
        {
            var validationsCompleted = false;
            _webElementLoggerMock.Setup(logger =>
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
            _webDriverLoggerMock.Verify(logger =>
                    logger.Log(It.IsAny<Type>(), It.IsAny<Level>(), It.IsAny<object>(), It.IsAny<Exception>()),
                Times.AtLeastOnce(), "Failed to log a trace message!");
        }

        [Test]
        public void ShouldLogOnFindElement()
        {
            var by = By.Id("it");
            var webElementMock = new Mock<IWebElement>();
            WebDriverMock.Setup(webDriver => webDriver.FindElement(by)).Returns(webElementMock.Object);

            WebDriverWrapper.FindElement(by);

            _webDriverLoggerMock.Verify(logger => logger.Log(It.IsAny<Type>(), Level.Trace, It.IsAny<object>(), null));
        }
    }
}