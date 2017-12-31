using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Positions.DetailExtractors;

namespace Sonneville.FidelityWebDriver.Tests.Positions.DetailExtractors
{
    [TestFixture]
    public class PositionTickerExtractorTests
    {
        [Test]
        [TestCase("asdf", "garbage fund")]
        public void ShouldExtractCoreTickerAndDescription(string expectedCoreTicker, string expectedDescription)
        {
            var tdElements = new List<IWebElement> {SetupSymbolTd(true, expectedCoreTicker, expectedDescription)};

            var extractor = new PositionTickerExtractor();
            var actualCoreTicker = extractor.ExtractCoreTicker(tdElements);
            var actualDescription = extractor.ExtractDescription(tdElements);

            Assert.AreEqual(expectedCoreTicker, actualCoreTicker);
            Assert.AreEqual(expectedDescription, actualDescription);
        }

        [Test]
        [TestCase("blah", "super fund")]
        public void ShouldExtractNonCoreTickerAndDescription(string expectedNonCoreTicker, string expectedDescription)
        {
            var tdElements = new List<IWebElement> {SetupSymbolTd(false, expectedNonCoreTicker, expectedDescription)};

            var extractor = new PositionTickerExtractor();
            var actualNonCoreTicker = extractor.ExtractNonCoreTicker(tdElements);
            var actualDescription = extractor.ExtractDescription(tdElements);

            Assert.AreEqual(expectedNonCoreTicker, actualNonCoreTicker);
            Assert.AreEqual(expectedDescription, actualDescription);
        }

        private IWebElement SetupSymbolTd(bool isCore, string ticker, string description)
        {
            var symbolSpanMock = new Mock<IWebElement>();
            symbolSpanMock.Setup(span => span.Text).Returns(isCore
                ? $"{ticker}**"
                : ticker);

            var descriptionSpanMock = new Mock<IWebElement>();
            descriptionSpanMock.Setup(span => span.Text).Returns(description);

            var tdMock = new Mock<IWebElement>();
            tdMock.Setup(td => td.FindElement(By.ClassName("stock-symbol"))).Returns(symbolSpanMock.Object);
            tdMock.Setup(td => td.FindElement(By.ClassName("stock-name"))).Returns(descriptionSpanMock.Object);
            return tdMock.Object;
        }
    }
}