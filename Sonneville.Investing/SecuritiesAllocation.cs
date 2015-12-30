using System.Collections;
using System.Collections.Generic;

namespace Sonneville.Investing
{
    public interface ISecuritiesAllocation : IEnumerable<KeyValuePair<string, double>>
    {
    }

    public class SecuritiesAllocation : ISecuritiesAllocation
    {
        private readonly Dictionary<string, double> _ratesByTicker;

        public SecuritiesAllocation(Dictionary<string, double> ratesByTicker)
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