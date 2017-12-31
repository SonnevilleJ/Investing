using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Positions.DetailExtractors;

namespace Sonneville.FidelityWebDriver.Tests.Positions.DetailExtractors
{
    [TestFixture]
    public class PositionCoreExtractorTests
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ShouldExtractCore(bool expected)
        {
            var tdElements = new List<IWebElement> {SetupSymbolTd(expected)};

            var isCore = new PositionCoreExtractor().ExtractIsCore(tdElements.AsReadOnly());

            Assert.AreEqual(expected, isCore);
        }

        private IWebElement SetupSymbolTd(bool isCore)
        {
            var ticker = isCore
                ? "ticker**"
                : "ticker";

            var symbolSpanMock = new Mock<IWebElement>();
            symbolSpanMock.Setup(span => span.Text).Returns(ticker);

            var tdMock = new Mock<IWebElement>();
            tdMock.Setup(td => td.FindElement(By.ClassName("stock-symbol"))).Returns(symbolSpanMock.Object);
            return tdMock.Object;
        }
    }
}