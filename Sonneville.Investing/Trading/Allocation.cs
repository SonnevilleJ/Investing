using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing.Trading
{
    public class Allocation
    {
        public static Allocation FromDictionary(IReadOnlyDictionary<string, decimal> accountDictionary)
        {
            if (Math.Abs(accountDictionary.Sum(kvp => kvp.Value) - 1m) > 0.0001m)
                throw new ArgumentException("Allocated percentages must total 100%!");
            if (accountDictionary.Values.Any(value => value <= 0))
                throw new ArgumentException("Invalid allocated percentage found!");

            return new Allocation(accountDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        private readonly IReadOnlyDictionary<string, decimal> _accountDictionary;

        private Allocation(IReadOnlyDictionary<string, decimal> accountDictionary)
        {
            _accountDictionary = accountDictionary;
        }

        public decimal GetPercent(string ticker)
        {
            return _accountDictionary.ContainsKey(ticker)
                ? _accountDictionary[ticker]
                : 0;
        }

        public decimal GetDollars(string ticker, decimal dollars)
        {
            return GetPercent(ticker)*dollars;
        }

        public IReadOnlyDictionary<string, decimal> ToDictionary()
        {
            return _accountDictionary;
        }
    }
}