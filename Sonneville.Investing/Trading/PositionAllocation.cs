using System;
using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing.Trading
{
    [Serializable]
    public class PositionAllocation
    {
        public static PositionAllocation FromDictionary(IReadOnlyDictionary<string, decimal> positionsDictionary)
        {
            if (Math.Abs(positionsDictionary.Sum(kvp => kvp.Value) - 1m) > 0.0001m)
                throw new ArgumentException("Total allocation must total 100%");
            if (positionsDictionary.Values.Any(value => value <= 0))
                throw new ArgumentException("Invalid allocated percentage found!");

            return new PositionAllocation(positionsDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        public static PositionAllocation ForMultiAccount(IReadOnlyDictionary<string, decimal> positionsDictionary)
        {
            if (positionsDictionary.Values.Any(value => value <= 0) || positionsDictionary.Values.Sum() > 1)
                throw new ArgumentException("Invalid allocated percentage found!");

            return new PositionAllocation(positionsDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        private readonly IReadOnlyDictionary<string, decimal> _positionsDictionary;

        private PositionAllocation(IReadOnlyDictionary<string, decimal> positionsDictionary)
        {
            _positionsDictionary = positionsDictionary;
        }

        public decimal GetPercent(string ticker)
        {
            return _positionsDictionary.ContainsKey(ticker)
                ? _positionsDictionary[ticker]
                : 0;
        }

        public decimal GetDollars(string ticker, decimal dollars)
        {
            return GetPercent(ticker)*dollars;
        }

        public Dictionary<string, decimal> ToDictionary()
        {
            return _positionsDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}