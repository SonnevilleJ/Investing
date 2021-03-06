using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Sonneville.Selenium.log4net.Test
{
    [TestFixture]
    public class ExceptionReportingWebDriverTests : WebDriverTestsBase<ExceptionReportingWebDriver>
    {
        private Mock<IExceptionReportGenerator> _mockExceptionReportGenerator;

        [SetUp]
        public override void Setup()
        {
            _mockExceptionReportGenerator = new Mock<IExceptionReportGenerator>();

            base.Setup();
        }

        protected override ExceptionReportingWebDriver InstantiateWebDriverWrapper(IWebDriver webDriver)
        {
            return new ExceptionReportingWebDriver(_mockExceptionReportGenerator.Object, webDriver);
        }

        protected override void AssertSubjectInvokesDependencyCorrectly(
            IWebElement wrapper,
            Mock<IWebElement> innerMock,
            Action<IWebElement> wrapperAction,
            Expression<Action<IWebElement>> expectedInnerInvocation)
        {
            var exception = new Exception("some exception");
            innerMock.Setup(expectedInnerInvocation)
                .Throws(exception);

            var thrownException = Assert.Throws<Exception>(() => wrapperAction(wrapper));

            Assert.AreEqual(exception, thrownException);
            _mockExceptionReportGenerator.Verify(generator => generator.DocumentException(exception));
        }
    }
}