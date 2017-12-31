using System;
using System.Collections.Generic;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Utilities;

namespace Sonneville.FidelityWebDriver.Transactions.CSV
{
    public interface ITransactionMapper
    {
        IFidelityTransaction CreateTransaction(string row, IDictionary<FidelityCsvColumn, int> headers);
    }

    public class TransactionMapper : ITransactionMapper
    {
        private readonly ITransactionTypeMapper _transactionTypeMapper;

        public TransactionMapper(ITransactionTypeMapper transactionTypeMapper)
        {
            _transactionTypeMapper = transactionTypeMapper;
        }

        public IFidelityTransaction CreateTransaction(string row, IDictionary<FidelityCsvColumn, int> headers)
        {
            var values = row.Split(',');
            var actionText = ParseStringField(values[headers[FidelityCsvColumn.Action]]);
            return new FidelityTransaction
            {
                RunDate = ParseDateField(values[headers[FidelityCsvColumn.RunDate]]),
                AccountNumber = ParseStringField(values[headers[FidelityCsvColumn.Account]]),
                Action = actionText,
                Type = _transactionTypeMapper.ClassifyDescription(actionText),
                Symbol = ParseStringField(values[headers[FidelityCsvColumn.Symbol]]),
                SecurityDescription = ParseStringField(values[headers[FidelityCsvColumn.SecurityDescription]]),
                SecurityType = ParseStringField(values[headers[FidelityCsvColumn.SecurityType]]),
                Quantity = ParseDecimalField(values[headers[FidelityCsvColumn.Quantity]]),
                Price = ParseDecimalField(values[headers[FidelityCsvColumn.Price]]),
                Commission = ParseDecimalField(values[headers[FidelityCsvColumn.Commission]]),
                Fees = ParseDecimalField(values[headers[FidelityCsvColumn.Fees]]),
                AccruedInterest = ParseDecimalField(values[headers[FidelityCsvColumn.AccruedInterest]]),
                Amount = ParseDecimalField(values[headers[FidelityCsvColumn.Amount]]),
                SettlementDate = ParseDateField(values[headers[FidelityCsvColumn.SettlementDate]]),
            };
        }

        private decimal? ParseDecimalField(string decimalString)
        {
            return string.IsNullOrWhiteSpace(decimalString)
                ? new decimal?()
                : NumberParser.ParseDecimal(decimalString.Trim());
        }

        private string ParseStringField(string rawString)
        {
            return rawString.Trim();
        }

        private DateTime? ParseDateField(string dateString)
        {
            return string.IsNullOrWhiteSpace(dateString)
                ? new DateTime?()
                : DateTime.Parse(dateString);
        }
    }
}