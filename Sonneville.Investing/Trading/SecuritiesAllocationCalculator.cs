﻿using System;
using System.Collections.Generic;

namespace Sonneville.Investing.Trading
{
    public interface ISecuritiesAllocation
    {
        decimal CalculateAllocation(string ticker, IEnumerable<Position> positions);
    }

    public class SecuritiesAllocationCalculator : ISecuritiesAllocation
    {
        public decimal CalculateAllocation(string ticker, IEnumerable<Position> positions)
        {
            var foundTickers = new HashSet<string>();
            Position matchingPosition = null;
            decimal totalValue = 0;
            foreach (var position in positions)
            {
                if (!foundTickers.Add(position.Ticker))
                {
                    const string errorMessage = "Cannot calculate allocation when multiple positions exist with same ticker!";
                    throw new ArgumentException(errorMessage, nameof(positions));
                }
                if (position.Ticker == ticker)
                {
                    matchingPosition = position;
                }
                totalValue += position.Value;
            }
            return matchingPosition == null ? 0 : matchingPosition.Value/totalValue;
        }
    }
}