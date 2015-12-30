using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing.Trading
{
    public interface ISecuritiesAllocation
    {
        decimal GetAllocation(string ticker, IList<Position> positions);
    }

    public class SecuritiesAllocationCalculator : ISecuritiesAllocation
    {
        public decimal GetAllocation(string ticker, IList<Position> positions)
        {
            var matchingPosition = positions.SingleOrDefault(position => position.Ticker == ticker);
            return matchingPosition != null
                ? matchingPosition.Value/positions.Sum(position => position.Value)
                : 0;
        }
    }
}