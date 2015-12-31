﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sonneville.FidelityWebDriver.Data;
using Position = Sonneville.Investing.Trading.Position;

namespace Sonneville.Investing.PortfolioManager.FidelityWebDriver
{
    public interface IPositionMapper
    {
        Position Map(IPosition extractedPosition);

        IList<Position> Map(IEnumerable<IPosition> extractedPosition);
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

        public IList<Position> Map(IEnumerable<IPosition> extractedPosition)
        {
            return extractedPosition.Select(Map).ToList();
        }
    }
}