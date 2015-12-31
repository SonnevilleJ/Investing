using System;
using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing.Trading
{
    public interface ISecuritiesAllocationCalculator
    {
        decimal CalculateAllocation(Position toAllocate, IEnumerable<Position> portfolio);

        IDictionary<Position, decimal> CalculateAllocations(IReadOnlyList<Position> positions);

        decimal CalculateAllocation(Position toAllocate, IEnumerable<TradingAccount> tradingAccounts);

        IDictionary<TradingAccount, IDictionary<Position, decimal>> CalculateAllocations(
            IReadOnlyList<TradingAccount> tradingAccounts);
    }

    public class SecuritiesAllocationCalculator : ISecuritiesAllocationCalculator
    {
        public decimal CalculateAllocation(Position toAllocate, IEnumerable<Position> portfolio)
        {
            var foundInList = false;
            decimal sum = 0;
            foreach (var position in portfolio)
            {
                if (!foundInList && position == toAllocate) foundInList = true;
                sum += position.Value;
            }
            if (!foundInList)
            {
                const string message = "Cannot calculate allocation for position not listed in the portfolio!";
                throw new KeyNotFoundException(message);
            }
            return toAllocate.Value/sum;
        }

        public IDictionary<Position, decimal> CalculateAllocations(IReadOnlyList<Position> positions)
        {
            var totalValue = positions.Sum(position => position.Value);

            return positions.ToDictionary(position => position, position => position.Value/totalValue);
        }

        public decimal CalculateAllocation(Position toAllocate, IEnumerable<TradingAccount> tradingAccounts)
        {
            return CalculateAllocation(toAllocate, tradingAccounts.SelectMany(account => account.Positions));
        }

        public IDictionary<TradingAccount, IDictionary<Position, decimal>> CalculateAllocations(
            IReadOnlyList<TradingAccount> tradingAccounts)
        {
            var totalValue = tradingAccounts.Sum(account => account.Positions.Sum(position => position.Value));
            return tradingAccounts.ToDictionary<TradingAccount, TradingAccount, IDictionary<Position, decimal>>(
                account => account,
                account => account.Positions.ToDictionary(position => position, position => position.Value/totalValue));
        }
    }
}