using System;
using System.Collections.Generic;
using System.Linq;
using Sonneville.Fidelity.WebDriver.Data;
using Position = Sonneville.Investing.Trading.Position;

namespace Sonneville.Fidelity.Shell.FidelityWebDriver
{
    public interface IPositionMapper
    {
        Position Map(IPosition extractedPosition);

        IEnumerable<Position> Map(IEnumerable<IPosition> extractedPosition);
    }

    public class PositionMapper : IPositionMapper
    {
        public Position Map(IPosition extractedPosition)
        {
            return new Position
            {
                DateTime = DateTime.Today,
                Ticker = extractedPosition.Ticker,
                IsCore = extractedPosition.IsCore,
                IsMargin = extractedPosition.IsMargin,
                Shares = extractedPosition.Quantity,
                PerSharePrice = extractedPosition.LastPrice,
            };
        }

        public IEnumerable<Position> Map(IEnumerable<IPosition> extractedPosition)
        {
            return extractedPosition.Select(Map);
        }
    }
}