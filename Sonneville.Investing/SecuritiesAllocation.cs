using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing
{
    public interface ISecuritiesAllocation : IEnumerable<KeyValuePair<string, double>>
    {
        double GetAmount(string ticker);

        IReadOnlyDictionary<string, double> ToDictionary();
    }

    public class SecuritiesAllocation : ISecuritiesAllocation
    {
        public static SecuritiesAllocation FromDictionary(Dictionary<string, double> ratesByTicker)
        {
            if (Math.Abs(ratesByTicker.Sum(rate => rate.Value) - 1) > 0.01)
                throw new InvalidOperationException("Allocations must total 100%!");
            if (ratesByTicker.Any(rate => rate.Value < 0))
                throw new InvalidOperationException("Allocations must be greater than 0%!");
            return new SecuritiesAllocation(ratesByTicker);
        }

        private readonly Dictionary<string, double> _ratesByTicker;

        private SecuritiesAllocation(Dictionary<string, double> ratesByTicker)
        {
            _ratesByTicker = ratesByTicker;
        }

        public double GetAmount(string ticker)
        {
            return _ratesByTicker.ContainsKey(ticker)
                ? _ratesByTicker[ticker]
                : 0;
        }

        public IReadOnlyDictionary<string, double> ToDictionary()
        {
            return new Dictionary<string, double>(_ratesByTicker);
        }

        public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
        {
            return _ratesByTicker.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}