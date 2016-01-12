using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing.Trading
{
    public class AccountAllocation
    {
        public static AccountAllocation FromDictionary(IReadOnlyDictionary<string, PositionAllocation> accountDictionary)
        {
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