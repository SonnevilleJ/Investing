using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Trading;

namespace Sonneville.Fidelity.Shell
{
    public interface IAllocationDifferencer
    {
        PositionAllocation CalculateDifference(PositionAllocation minuend, PositionAllocation subtrahend);

        AccountAllocation CalculateDifference(AccountAllocation minuend, AccountAllocation subtrahend);
    }

    public class AllocationDifferencer : IAllocationDifferencer
    {
        public PositionAllocation CalculateDifference(PositionAllocation minuend, PositionAllocation subtrahend)
        {
            var dictionary = new Dictionary<string, decimal>();
            var minuendDictionary = minuend.ToDictionary();
            var subtrahendDictionary = subtrahend.ToDictionary();
            foreach (var mkvp in minuendDictionary)
            {
                decimal sValue = 0;
                if (subtrahendDictionary.ContainsKey(mkvp.Key))
                {
                    sValue = subtrahendDictionary[mkvp.Key];
                }
                dictionary.Add(mkvp.Key, mkvp.Value - sValue);
            }
            foreach (var skvp in subtrahendDictionary.Where(skvp => !minuendDictionary.ContainsKey(skvp.Key)))
            {
                dictionary.Add(skvp.Key, 0 - skvp.Value);
            }
            return PositionAllocation.FromDictionary(dictionary);
        }

        public AccountAllocation CalculateDifference(AccountAllocation minuend, AccountAllocation subtrahend)
        {
            var subtrahendDictionary = subtrahend.ToDictionary();
            var accountDictionary = minuend.ToDictionary().ToDictionary(
                kvp => kvp.Key,
                kvp => CalculateDifference(kvp.Value, subtrahendDictionary[kvp.Key]));

            return AccountAllocation.FromDictionary(accountDictionary);
        }
    }
}