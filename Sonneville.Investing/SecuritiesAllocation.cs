using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing
{
    public interface ISecuritiesAllocation : IEnumerable<KeyValuePair<string, decimal>>
    {
        decimal GetAmount(string ticker);

        IReadOnlyDictionary<string, decimal> ToDictionary();
    }

    public class SecuritiesAllocation : ISecuritiesAllocation
    {
        public static SecuritiesAllocation FromDictionary(Dictionary<string, decimal> ratesByTicker)
        {
            if (Math.Abs(ratesByTicker.Sum(rate => rate.Value) - 1) > 0.01m)
                throw new InvalidOperationException("Allocations must total 100%!");
            if (ratesByTicker.Any(rate => rate.Value < 0))
                throw new InvalidOperationException("Allocations must be greater than 0%!");
            return new SecuritiesAllocation(ratesByTicker);
        }

        private readonly Dictionary<string, decimal> _ratesByTicker;

        private SecuritiesAllocation(Dictionary<string, decimal> ratesByTicker)
        {
            _ratesByTicker = ratesByTicker;
        }

        public decimal GetAmount(string ticker)
        {
            return _ratesByTicker.ContainsKey(ticker)
                ? _ratesByTicker[ticker]
                : 0;
        }

        public IReadOnlyDictionary<string, decimal> ToDictionary()
        {
            return new Dictionary<string, decimal>(_ratesByTicker);
        }

        public IEnumerator<KeyValuePair<string, decimal>> GetEnumerator()
        {
            return _ratesByTicker.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}