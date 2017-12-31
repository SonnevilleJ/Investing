using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Positions.DetailExtractors;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Tests.Positions.DetailExtractors
{
    [TestFixture]
    public class PositionCurrentValueExtractorTests
    {
        [Test]
        [TestCase("$0.00")]
        [TestCase("$85.43")]
        [TestCase("$1,234.56")]
        public void ShouldExtractCurrentValue(string currentValueText)
        {
            var tdElements = SetupTdElements(currentValueText);

            var actualCurrentValue = new PositionCurrentValueExtractor().ExtractCurrentValue(tdElements);

            Assert.AreEqual(NumberParser.ParseDecimal(currentValueText), actualCurrentValue);
        }

        private List<IWebElement> SetupTdElements(string currentValueText)
        {
            return new List<IWebElement>
            {
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                SetupCurrentValueTd(currentValueText)
            };
        }

        private IWebElement SetupCurrentValueTd(string currentValueText)
        {
            var tdMock = new Mock<IWebElement>();
            tdMock.Setup(td => td.Text).Returns(currentValueText);
            return tdMock.Object;
        }
    }
}