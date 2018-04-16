using System;
using System.Collections.Generic;
using log4net;
using Sonneville.Fidelity.WebDriver.Utilities;
using Sonneville.Investing.Domain;

namespace Sonneville.Fidelity.WebDriver.Transactions.CSV
{
    public interface ITransactionMapper
    {
        ITransaction CreateTransaction(string row, IDictionary<FidelityCsvColumn, int> headers);
    }

    public class TransactionMapper : ITransactionMapper
    {
        private readonly ILog _log;
        private readonly ITransactionTypeMapper _transactionTypeMapper;

        public TransactionMapper(ILog log, ITransactionTypeMapper transactionTypeMapper)
        {
            _log = log;
            _transactionTypeMapper = transactionTypeMapper;
        }

        public ITransaction CreateTransaction(string row, IDictionary<FidelityCsvColumn, int> headers)
        {
            var values = row.Split(',');
            var actionText = ParseStringField(values[headers[FidelityCsvColumn.Action]]);
            var transaction = new Transaction
                {
                    RunDate = ParseDateField(values[headers[FidelityCsvColumn.RunDate]]),
                    AccountNumber = ParseAccountNumber(headers, values),
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
                    SettlementDate = ParseDateField(values[headers[FidelityCsvColumn.SettlementDate]])
                };
            _log.Debug($"Parsed transaction: {transaction}");
            return transaction;
        }

        private static string ParseAccountNumber(IDictionary<FidelityCsvColumn, int> headers, IReadOnlyList<string> values)
        {
            return headers.TryGetValue(FidelityCsvColumn.Account, out var column)
                ? values[column]
                : string.Empty;
        }

        private static decimal? ParseDecimalField(string decimalString)
        {
            return string.IsNullOrWhiteSpace(decimalString)
                ? new decimal?()
                : NumberParser.ParseDecimal(decimalString.Trim());
        }

        private static string ParseStringField(string rawString)
        {
            return rawString.Trim();
        }

        private static DateTime? ParseDateField(string dateString)
        {
            return string.IsNullOrWhiteSpace(dateString)
                ? new DateTime?()
                : DateTime.Parse(dateString);
        }
    }
}