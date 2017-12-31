using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Positions.DetailExtractors;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Tests.Positions.DetailExtractors
{
    [TestFixture]
    public class PositionCostBasisExtractorTests
    {
        [Test]
        [TestCase("$1.23", "$123.45")]
        [TestCase("$50.00", "$9,450.00")]
        public void Test(string costBasisPerShareString, string costBasisTotalString)
        {
            var tdElements = SetupTdElements(costBasisPerShareString, costBasisTotalString);

            var extractor = new PositionCostBasisExtractor();
            var actualPerShare = extractor.ExtractCostBasisPerShare(tdElements);
            var actualTotal = extractor.ExtractCostBasisTotal(tdElements);

            Assert.AreEqual(NumberParser.ParseDecimal(costBasisPerShareString), actualPerShare);
            Assert.AreEqual(NumberParser.ParseDecimal(costBasisTotalString), actualTotal);
        }

        [Test]
        [TestCase("--", "--")]
        [TestCase("--2", "--2")]
        [TestCase("n/a", "n/a")]
        [TestCase("$0.00", "$0.00")]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("\t", "\t")]
        public void ShouldExtractZeroForInvalidNumbers(string costBasisPerShareString, string costBasisTotalString)
        {
            var tdElements = SetupTdElements(costBasisPerShareString, costBasisTotalString);

            var extractor = new PositionCostBasisExtractor();
            var actualPerShare = extractor.ExtractCostBasisPerShare(tdElements);
            var actualTotal = extractor.ExtractCostBasisTotal(tdElements);

            Assert.AreEqual(0, actualPerShare);
            Assert.AreEqual(0, actualTotal);
        }

        private List<IWebElement> SetupTdElements(string costBasisPerShareString, string costBasisTotalString)
        {
            return new List<IWebElement>
            {
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                new Mock<IWebElement>(MockBehavior.Strict).Object,
                SetupCostBasisTd(costBasisPerShareString, costBasisTotalString)
            };
        }

        private IWebElement SetupCostBasisTd(string costBasisPerShareString, string costBasisTotalString)
        {
            var perShareSpanMock = new Mock<IWebElement>();
            perShareSpanMock.Setup(span => span.Text).Returns(costBasisPerShareString);

            var totalSpanMock = new Mock<IWebElement>();
            totalSpanMock.Setup(span => span.Text).Returns(costBasisTotalString);

            var totalDivMock = new Mock<IWebElement>();
            totalDivMock.Setup(div => div.FindElement(By.TagName("span"))).Returns(totalSpanMock.Object);

            var tdMock = new Mock<IWebElement>();
            tdMock.Setup(td => td.FindElements(By.ClassName("magicgrid--stacked-data-value")))
                .Returns(new List<IWebElement> { perShareSpanMock.Object, totalDivMock.Object }.AsReadOnly());

            return tdMock.Object;
        }
    }
}