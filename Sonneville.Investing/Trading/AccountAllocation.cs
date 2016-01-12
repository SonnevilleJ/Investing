using System;
using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing.Trading
{
    public class AccountAllocation
    {
        public static AccountAllocation FromDictionary(IReadOnlyDictionary<string, PositionAllocation> accountDictionary)
        {
            var totalAllocation = accountDictionary.Values.Sum(positions=>positions.ToDictionary().Values.Sum());
            if(Math.Abs(totalAllocation - 1) > 0.0001m)
                throw new ArgumentException("Total allocation must total 100% across all accounts!");

            return new AccountAllocation(accountDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        private readonly IReadOnlyDictionary<string, PositionAllocation> _accountDictionary;

        private AccountAllocation(IReadOnlyDictionary<string, PositionAllocation> accountDictionary)
        {
            _accountDictionary = accountDictionary;
        }

        public PositionAllocation GetPositionAllocation(string account)
        {
            return _accountDictionary[account];
        }

        public Dictionary<string, PositionAllocation> ToDictionary()
        {
            return _accountDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}