using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.FidelityWebDriver.Positions.DetailExtractors;

namespace Sonneville.FidelityWebDriver.Tests.Positions
{
    [TestFixture]
    public class PositionDetailsExtractorTests
    {
        private PositionDetailsExtractor _extractor;
        private List<IWebElement> _positionTableRows;
        private List<IPosition> _positions;
        private Mock<IPositionCoreExtractor> _coreExtractorMock;
        private Mock<IPositionTickerExtractor> _tickerExtractorMock;
        private Mock<IPositionLastPriceExtractor> _lastPriceExtractorMock;
        private Mock<IPositionTotalGainExtractor> _totalGainExtractorMock;
        private Mock<IPositionCurrentValueExtractor> _currentValueExtractorMock;
        private Mock<IPositionQuantityExtractor> _quantityExtractorMock;
        private Mock<IPositionCostBasisExtractor> _costBasisExtractorMock;

        [SetUp]
        public void Setup()
        {
            _coreExtractorMock = new Mock<IPositionCoreExtractor>();
            _tickerExtractorMock = new Mock<IPositionTickerExtractor>();
            _lastPriceExtractorMock = new Mock<IPositionLastPriceExtractor>();
            _totalGainExtractorMock = new Mock<IPositionTotalGainExtractor>();
            _currentValueExtractorMock = new Mock<IPositionCurrentValueExtractor>();
            _quantityExtractorMock = new Mock<IPositionQuantityExtractor>();
            _costBasisExtractorMock = new Mock<IPositionCostBasisExtractor>();

            _positions = SetupExpectedPositions();

            _positionTableRows = SetupPositionRows(_positions);

            _extractor = new PositionDetailsExtractor(_coreExtractorMock.Object,
                _tickerExtractorMock.Object, _lastPriceExtractorMock.Object,
                _totalGainExtractorMock.Object, _currentValueExtractorMock.Object,
                _quantityExtractorMock.Object, _costBasisExtractorMock.Object);
        }

        [Test]
        public void ShouldExtractPositionDetails()
        {
            var actuals = _extractor.ExtractPositionDetails(_positionTableRows).ToList();

            Assert.AreEqual(_positions.Count(), actuals.Count());
            foreach (var actual in actuals)
            {
                var matchingExpected = _positions.Single(expected =>
                    expected.Ticker == actual.Ticker && expected.IsMargin == actual.IsMargin);

                Assert.AreEqual(matchingExpected.Description, actual.Description);
                Assert.AreEqual(matchingExpected.IsCore, actual.IsCore);
                Assert.AreEqual(matchingExpected.LastPrice, actual.LastPrice);
                Assert.AreEqual(matchingExpected.TotalGainDollar, actual.TotalGainDollar);
                Assert.AreEqual(matchingExpected.TotalGainPercent, actual.TotalGainPercent);
                Assert.AreEqual(matchingExpected.CurrentValue, actual.CurrentValue);
                Assert.AreEqual(matchingExpected.Quantity, actual.Quantity);
                Assert.AreEqual(matchingExpected.CostBasisPerShare, actual.CostBasisPerShare);
                Assert.AreEqual(matchingExpected.CostBasisTotal, actual.CostBasisTotal);
            }
        }

        private List<IPosition> SetupExpectedPositions()
        {
            return new List<IPosition>
            {
                new Position("a", "b", true, false, 1.23m, 0, 0, 901.23m, 45.678m, 0, 0),
                new Position("c", "d", false, false, 3.21m, 2.34m, 0.56m, 246m, 45.678m, 1.23m, 123456.78m),
                new Position("c", "d", false, true, 4.56m, 7.89m, 0.56m, 246m, 45.678m, 1.23m, 123456.78m),
            };
        }

        private List<IWebElement> SetupPositionRows(IEnumerable<IPosition> positions)
        {
            return positions.SelectMany(CreatePositionRows).ToList();
        }

        private IEnumerable<IWebElement> CreatePositionRows(IPosition position)
        {
            var tdElements = new List<IWebElement>
            {
                new Mock<IWebElement>().Object,
                new Mock<IWebElement>().Object,
                new Mock<IWebElement>().Object,
                new Mock<IWebElement>().Object,
                new Mock<IWebElement>().Object,
                new Mock<IWebElement>().Object,
                new Mock<IWebElement>().Object,
            }.AsReadOnly();

            _coreExtractorMock.Setup(extractor => extractor.ExtractIsCore(tdElements))
                .Returns(position.IsCore);

            if (position.IsCore)
            {
                _tickerExtractorMock.Setup(extractor => extractor.ExtractCoreTicker(tdElements))
                    .Returns(position.Ticker);
            }
            else
            {
                _tickerExtractorMock.Setup(extractor => extractor.ExtractNonCoreTicker(tdElements))
                    .Returns(position.Ticker);
            }

            _tickerExtractorMock.Setup(extractor => extractor.ExtractDescription(tdElements))
                .Returns(position.Description);

            _lastPriceExtractorMock.Setup(extractor => extractor.ExtractLastPrice(tdElements))
                .Returns(position.LastPrice);

            _totalGainExtractorMock.Setup(extractor => extractor.ExtractTotalGainDollar(tdElements))
                .Returns(position.TotalGainDollar);

            _totalGainExtractorMock.Setup(extractor => extractor.ExtractTotalGainPercent(tdElements))
                .Returns(position.TotalGainPercent);

            _currentValueExtractorMock.Setup(extractor => extractor.ExtractCurrentValue(tdElements))
                .Returns(position.CurrentValue);

            _quantityExtractorMock.Setup(extractor => extractor.ExtractQuantity(tdElements))
                .Returns(position.Quantity);

            _quantityExtractorMock.Setup(extractor => extractor.ExtractMargin(tdElements))
                .Returns(position.IsMargin);

            _costBasisExtractorMock.Setup(extractor => extractor.ExtractCostBasisPerShare(tdElements))
                .Returns(position.CostBasisPerShare);

            _costBasisExtractorMock.Setup(extractor => extractor.ExtractCostBasisTotal(tdElements))
                .Returns(position.CostBasisTotal);

            var normalRowMock = new Mock<IWebElement>();
            normalRowMock.Setup(row => row.GetAttribute("class")).Returns("normal-row");
            normalRowMock.Setup(row => row.FindElements(By.XPath("./td"))).Returns(tdElements);
            yield return normalRowMock.Object;

            var contentRowMock = new Mock<IWebElement>();
            contentRowMock.Setup(row => row.GetAttribute("class")).Returns("content-row");
            yield return contentRowMock.Object;
        }
    }
}