using System.Collections.Generic;
using System.Linq;

namespace Sonneville.FidelityWebDriver.Transactions.CSV
{
    public interface IFidelityCsvColumnMapper
    {
        FidelityCsvColumn GetHeader(string text);

        IDictionary<FidelityCsvColumn, int> GetColumnMappings(string headerRow);
    }

    public class FidelityCsvColumnMapper : IFidelityCsvColumnMapper
    {
        public FidelityCsvColumn GetHeader(string text)
        {
            var trimmed = text.Trim();
            switch (trimmed)
            {
                case "Run Date":
                    return FidelityCsvColumn.RunDate;
                case "Account":
                    return FidelityCsvColumn.Account;
                case "Action":
                    return FidelityCsvColumn.Action;
                case "Symbol":
                    return FidelityCsvColumn.Symbol;
                case "Security Description":
                    return FidelityCsvColumn.SecurityDescription;
                case "Security Type":
                    return FidelityCsvColumn.SecurityType;
                case "Quantity":
                    return FidelityCsvColumn.Quantity;
                case "Price":
                    return FidelityCsvColumn.Price;
                case "Commission":
                    return FidelityCsvColumn.Commission;
                case "Fees":
                    return FidelityCsvColumn.Fees;
                case "Accrued Interest":
                    return FidelityCsvColumn.AccruedInterest;
                case "Amount":
                    return FidelityCsvColumn.Amount;
                case "Settlement Date":
                    return FidelityCsvColumn.SettlementDate;
                default:
                    return FidelityCsvColumn.Unknown;
            }
        }

        public IDictionary<FidelityCsvColumn, int> GetColumnMappings(string headerRow)
        {
            var columns = headerRow.Split(',');
            var count = 0;
            return columns.Select(s => new KeyValuePair<FidelityCsvColumn, int>(GetHeader(s), count++))
                    .Where(header => header.Key != FidelityCsvColumn.Unknown)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}