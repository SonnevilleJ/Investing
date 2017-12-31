using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Positions.DetailExtractors;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Tests.Positions.DetailExtractors
{
    [TestFixture]
    public class PositionLastPriceExtractorTests
    {
        [Test]
        [TestCase("$1.00")]
        [TestCase("$8.08")]
        [TestCase("$49.971")]
        public void ShouldExtractLastPrice(string lastPriceString)
        {
            var tdElements = SetupTdElements(lastPriceString);

            var actualPrice = new PositionLastPriceExtractor().ExtractLastPrice(tdElements);
            
            Assert.AreEqual(NumberParser.ParseDecimal(lastPriceString), actualPrice);
        }

        [Test]
        [TestCase("--")]
        public void ShouldExtractZeroForInvalidPrices(string lastPriceString)
        {
            var tdElements = SetupTdElements(lastPriceString);

            var actualPrice = new PositionLastPriceExtractor().ExtractLastPrice(tdElements);
            
            Assert.AreEqual(0m, actualPrice);
        }

        private List<IWebElement> SetupTdElements(string lastPriceString)
        {
            return new List<IWebElement>
            {
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                SetupLastPriceTd(lastPriceString),
            };
        }

        private IWebElement SetupLastPriceTd(string lastPriceString)
        {
            var spanMock = new Mock<IWebElement>();
            spanMock.Setup(span => span.Text).Returns(lastPriceString);

            var tdMock = new Mock<IWebElement>();
            tdMock.Setup(td => td.FindElements(By.ClassName("magicgrid--stacked-data-value")))
                .Returns(new List<IWebElement> { spanMock.Object }.AsReadOnly());
            return tdMock.Object;
        }
    }
}