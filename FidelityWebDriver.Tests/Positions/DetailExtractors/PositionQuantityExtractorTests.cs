using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Positions.DetailExtractors;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Tests.Positions.DetailExtractors
{
    [TestFixture]
    public class PositionQuantityExtractorTests
    {
        [Test]
        [TestCase(false, "1.000")]
        [TestCase(false, "1,259.461")]
        [TestCase(false, "641.370")]
        [TestCase(true, "1.000")]
        [TestCase(true, "1,259.461")]
        [TestCase(true, "641.370")]
        public void ShouldExtractMarginAndQuantity(bool isMargin, string quantityString)
        {
            var tdElements = SetupTdElements(isMargin, quantityString);

            var actualMargin = new PositionQuantityExtractor().ExtractMargin(tdElements);
            var actualQuantity = new PositionQuantityExtractor().ExtractQuantity(tdElements);

            Assert.AreEqual(isMargin, actualMargin);
            Assert.AreEqual(NumberParser.ParseDecimal(quantityString), actualQuantity);
        }

        private List<IWebElement> SetupTdElements(bool isMargin, string quantityString)
        {
            return new List<IWebElement>
            {
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                SetupQuantityTd(isMargin, quantityString)
            };
        }

        private IWebElement SetupQuantityTd(bool isMargin, string quantityString)
        {
            var tdText = isMargin
                ? $"\"{quantityString}\"<br>\"(Margin)\""
                : quantityString;

            var tdMock = new Mock<IWebElement>();
            tdMock.Setup(td => td.Text).Returns(tdText);
            return tdMock.Object;
        }
    }
}