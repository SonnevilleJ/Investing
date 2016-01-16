using System.Collections.Generic;
using System.Linq;

namespace Sonneville.Investing.Trading
{
    public interface ISecuritiesAllocationCalculator
    {
        decimal CalculateAllocation(Position toAllocate, IEnumerable<Position> portfolio);

        PositionAllocation CalculatePositionAllocation(IReadOnlyList<Position> positions);

        decimal CalculateAllocation(Position toAllocate, IEnumerable<TradingAccount> tradingAccounts);

        AccountAllocation CalculateAccountAllocation(IReadOnlyList<TradingAccount> tradingAccounts);
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

        public PositionAllocation CalculatePositionAllocation(IReadOnlyList<Position> positions)
        {
            var totalValue = positions.Sum(position => position.Value);
            return PositionAllocation.FromDictionary(CreatePositionsDictionary(positions, totalValue));
        }

        public decimal CalculateAllocation(Position toAllocate, IEnumerable<TradingAccount> tradingAccounts)
        {
            return CalculateAllocation(toAllocate, tradingAccounts.SelectMany(account => account.Positions));
        }

        public AccountAllocation CalculateAccountAllocation(IReadOnlyList<TradingAccount> tradingAccounts)
        {
            var totalValue = tradingAccounts.Sum(account => account.Positions.Sum(position => position.Value));
            return AccountAllocation.FromDictionary(tradingAccounts.ToDictionary(
                account => account.AccountId,
                account => PositionAllocation.FromDictionary(CreatePositionsDictionary(account.Positions, totalValue))));
        }

        private static Dictionary<string, decimal> CreatePositionsDictionary(IReadOnlyList<Position> positions, decimal totalValue)
        {
            var dictionary = new Dictionary<string, decimal>();
            foreach (var ticker in positions.Select(position => position.Ticker).Distinct())
            {
                var value = positions.Where(position => position.Ticker == ticker).Sum(position => position.Value);
                dictionary.Add(ticker, value / totalValue);
            }
            return dictionary;
        }
    }
}