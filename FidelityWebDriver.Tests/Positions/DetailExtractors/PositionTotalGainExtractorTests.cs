using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Positions.DetailExtractors;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Tests.Positions.DetailExtractors
{
    [TestFixture]
    public class PositionTotalGainExtractorTests
    {
        [Test]
        [TestCase("$0.00", "0.00%")]
        [TestCase("+$12.77", "0.47%")]
        [TestCase("-$64.23", "-11.25%")]
        public void ShouldExtractDollarAndPercentChange(string totalGainDollarString, string totalGainPercentString)
        {
            var tdElements = SetupTdElements(totalGainDollarString, totalGainPercentString);

            var actualDollarGain = new PositionTotalGainExtractor().ExtractTotalGainDollar(tdElements);
            var actualPercentGain = new PositionTotalGainExtractor().ExtractTotalGainPercent(tdElements);

            Assert.AreEqual(NumberParser.ParseDecimal(totalGainDollarString), actualDollarGain);
            Assert.AreEqual(NumberParser.ParseDecimal(totalGainPercentString.Replace("%", ""))/100, actualPercentGain);
        }

        [Test]
        [TestCase("--", "--")]
        [TestCase("--2", "--2")]
        [TestCase("n/a", "n/a")]
        public void ShouldExtractZeroForInvalidAmounts(string totalGainDollarString, string totalGainPercentString)
        {
            var tdElements = SetupTdElements(totalGainDollarString, totalGainPercentString);

            var actualDollarGain = new PositionTotalGainExtractor().ExtractTotalGainDollar(tdElements);
            var actualPercentGain = new PositionTotalGainExtractor().ExtractTotalGainPercent(tdElements);

            Assert.AreEqual(0m, actualDollarGain);
            Assert.AreEqual(0m, actualPercentGain);
        }

        private List<IWebElement> SetupTdElements(string totalGainDollarString, string totalGainPercentString)
        {
            return new List<IWebElement>
            {
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                SetupTotalGainTd(totalGainDollarString, totalGainPercentString),
            };
        }

        private IWebElement SetupTotalGainTd(string totalGainDollarString, string totalGainPercentString)
        {
            var span1 = new Mock<IWebElement>();
            span1.Setup(span => span.Text).Returns(totalGainDollarString);

            var span2 = new Mock<IWebElement>();
            span2.Setup(span => span.Text).Returns(totalGainPercentString);

            var tdMock = new Mock<IWebElement>();
            tdMock.Setup(td => td.FindElements(By.ClassName("magicgrid--stacked-data-value")))
                .Returns(new List<IWebElement> {span1.Object, span2.Object}.AsReadOnly());
            return tdMock.Object;
        }
    }
}