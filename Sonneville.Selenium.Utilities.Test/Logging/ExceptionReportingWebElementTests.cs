using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.Selenium.Utilities.Logging;

namespace Sonneville.Selenium.Utilities.Test.Logging
{
    [TestFixture]
    public class ExceptionReportingWebElementTests
    {
        private ExceptionReportingWebElement _webElement;
        private Mock<IWebElement> _mockWebElement;
        private Mock<IExceptionReportGenerator> _mockReportGenerator;

        [SetUp]
        public void Setup()
        {
            _mockWebElement = new Mock<IWebElement>();
            
            _mockReportGenerator = new Mock<IExceptionReportGenerator>();

            _webElement = new ExceptionReportingWebElement(
                _mockWebElement.Object,
                _mockReportGenerator.Object);
        }

        [Test]
        public void ClickShouldPassExceptionsToReportGenerator()
        {
            AssertWebElementDocumentsAndRethrowsExceptions(element => element.Click());
        }

        [Test]
        public void ClearShouldPassExceptionsToReportGenerator()
        {
            AssertWebElementDocumentsAndRethrowsExceptions(element => element.Clear());
        }

        [Test]
        public void SubmitShouldPassExceptionsToReportGenerator()
        {
            AssertWebElementDocumentsAndRethrowsExceptions(element => element.Submit());
        }

        [Test]
        public void SendKeysShouldPassExceptionsToReportGenerator()
        {
            AssertWebElementDocumentsAndRethrowsExceptions(element => element.SendKeys(""));
        }

        private void AssertWebElementDocumentsAndRethrowsExceptions(Expression<Action<IWebElement>> interaction)
        {
            var exception = new Exception("some exception");
            _mockWebElement.Setup(interaction)
                .Throws(exception);

            var thrownException = Assert.Throws<Exception>(() => interaction.Compile()(_webElement));

            Assert.AreEqual(exception, thrownException);
            _mockReportGenerator.Verify(generator => generator.DocumentException(exception));
        }
    }
}