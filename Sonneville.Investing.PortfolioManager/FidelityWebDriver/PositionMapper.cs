using System;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.PortfolioManager.FidelityWebDriver
{
    public class PositionMapper
    {
        public Position Map(Sonneville.FidelityWebDriver.Data.IPosition extractedPosition)
        {
            return new Position
            {
                DateTime = DateTime.Today,
                Ticker = extractedPosition.Ticker,
                Shares = extractedPosition.Quantity,
                PerSharePrice = extractedPosition.LastPrice,
            };
        }
    }
}