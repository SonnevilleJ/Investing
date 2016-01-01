using System.Collections.Generic;
using System.Linq;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.Utilities.Extensions;

namespace Sonneville.Investing.PortfolioManager
{
    public class AllocationDifferencer
    {
        public IDictionary<Position, decimal> CalculateDifference(IDictionary<Position, decimal> minuend,
            IDictionary<Position, decimal> subtrahend)
        {
            return minuend.ZipAll(subtrahend,
                (m, s) => new KeyValuePair<Position, decimal>(m.Key ?? s.Key, m.Value - s.Value))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}