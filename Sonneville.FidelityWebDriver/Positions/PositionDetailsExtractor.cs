using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Positions.DetailExtractors;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface IPositionDetailsExtractor
    {
        IEnumerable<IPosition> ExtractPositionDetails(IEnumerable<IWebElement> positionTableRows);
    }

    public class PositionDetailsExtractor : IPositionDetailsExtractor
    {
        private readonly IPositionCoreExtractor _positionCoreExtractor;
        private readonly IPositionTickerExtractor _positionTickerExtractor;
        private readonly IPositionLastPriceExtractor _positionLastPriceExtractor;
        private readonly IPositionTotalGainExtractor _positionTotalGainExtractor;
        private readonly IPositionCurrentValueExtractor _positionCurrentValueExtractor;
        private readonly IPositionQuantityExtractor _positionQuantityExtractor;
        private readonly IPositionCostBasisExtractor _positionCostBasisExtractor;

        public PositionDetailsExtractor(IPositionCoreExtractor positionCoreExtractor,
            IPositionTickerExtractor positionTickerExtractor,
            IPositionLastPriceExtractor positionLastPriceExtractor,
            IPositionTotalGainExtractor positionTotalGainExtractor,
            IPositionCurrentValueExtractor positionCurrentValueExtractor,
            IPositionQuantityExtractor positionQuantityExtractor,
            IPositionCostBasisExtractor positionCostBasisExtractor)
        {
            _positionCoreExtractor = positionCoreExtractor;
            _positionTickerExtractor = positionTickerExtractor;
            _positionLastPriceExtractor = positionLastPriceExtractor;
            _positionTotalGainExtractor = positionTotalGainExtractor;
            _positionCurrentValueExtractor = positionCurrentValueExtractor;
            _positionQuantityExtractor = positionQuantityExtractor;
            _positionCostBasisExtractor = positionCostBasisExtractor;
        }

        public IEnumerable<IPosition> ExtractPositionDetails(IEnumerable<IWebElement> positionTableRows)
        {
            var normalRows = positionTableRows.Where(row => row.GetAttribute("class").Contains("normal-row"));
            foreach (var positionTableRow in normalRows)
            {
                var tdElements = positionTableRow.FindElements(By.XPath("./td"));
                var position = new Position();

                position.IsCore = _positionCoreExtractor.ExtractIsCore(tdElements);
                position.Ticker = _positionCoreExtractor.ExtractIsCore(tdElements)
                    ? _positionTickerExtractor.ExtractCoreTicker(tdElements)
                    : _positionTickerExtractor.ExtractNonCoreTicker(tdElements);
                position.Description = _positionTickerExtractor.ExtractDescription(tdElements);
                position.LastPrice = _positionLastPriceExtractor.ExtractLastPrice(tdElements);
                position.TotalGainDollar = _positionTotalGainExtractor.ExtractTotalGainDollar(tdElements);
                position.TotalGainPercent = _positionTotalGainExtractor.ExtractTotalGainPercent(tdElements);
                position.CurrentValue = _positionCurrentValueExtractor.ExtractCurrentValue(tdElements);
                position.Quantity = _positionQuantityExtractor.ExtractQuantity(tdElements);
                position.IsMargin = _positionQuantityExtractor.ExtractMargin(tdElements);
                position.CostBasisPerShare = _positionCostBasisExtractor.ExtractCostBasisPerShare(tdElements);
                position.CostBasisTotal = _positionCostBasisExtractor.ExtractCostBasisTotal(tdElements);

                yield return position;
            }
        }
    }
}