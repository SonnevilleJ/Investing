using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing.Trading
{
    public class TradingAccountBalanceCalculator
    {
        public decimal CalculateBalance(TradingAccount tradingAccount)
        {
            return tradingAccount.Positions.Sum(position => position.Value);
        }

        public decimal CalculateBalance(IEnumerable<TradingAccount> tradingAccounts)
        {
            return tradingAccounts.Select(CalculateBalance).Sum();
        }
    }
}