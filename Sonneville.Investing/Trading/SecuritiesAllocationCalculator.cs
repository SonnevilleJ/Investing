using System;
using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing.Trading
{
    public interface ISecuritiesAllocationCalculator
    {
        decimal CalculateAllocation(Position toAllocate, IEnumerable<Position> portfolio);

        IDictionary<Position, decimal> CalculateAllocations(IReadOnlyList<Position> positions);
    }

    public class SecuritiesAllocationCalculator : ISecuritiesAllocationCalculator
    {
        public decimal CalculateAllocation(Position toAllocate, IEnumerable<Position> portfolio)
        {
            var foundInList = false;
            decimal sum = 0;
            foreach (var position in portfolio)
            {
                if (!foundInList && position == toAllocate) foundInList = true;
                sum += position.Value;
            }
            if (!foundInList)
                throw new ArgumentException("Cannot calculate allocation for position not listed in the portfolio!",
                    nameof(toAllocate));
            return toAllocate.Value/sum;
        }

        public IDictionary<Position, decimal> CalculateAllocations(IReadOnlyList<Position> positions)
        {
            var totalValue = positions.Sum(position => position.Value);

            return positions.ToDictionary(position => position, position => position.Value/totalValue);
        }
    }
}