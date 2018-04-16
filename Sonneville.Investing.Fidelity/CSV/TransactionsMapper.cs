using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Sonneville.Investing.Domain;

namespace Sonneville.Investing.Fidelity.CSV
{
    public interface ITransactionsMapper
    {
        IEnumerable<ITransaction> ParseCsv(string csvContent);
    }

    public class TransactionsMapper : ITransactionsMapper
    {
        private readonly ILog _log;
        private readonly IFidelityCsvColumnMapper _fidelityCsvColumnMapper;
        private readonly ITransactionMapper _transactionMapper;

        public TransactionsMapper(
            ILog log,
            IFidelityCsvColumnMapper fidelityCsvColumnMapper,
            ITransactionMapper transactionMapper)
        {
            _log = log;
            _fidelityCsvColumnMapper = fidelityCsvColumnMapper;
            _transactionMapper = transactionMapper;
        }

        public IEnumerable<ITransaction> ParseCsv(string csvContent)
        {
            var rows = csvContent.Trim().Split(new[] {"\n", "\r\n"}, StringSplitOptions.None);
            var columnIndices = _fidelityCsvColumnMapper.GetColumnMappings(rows.First());
            LogColumnIndices(columnIndices);
            return rows.Skip(1)
                .TakeWhile(line => !string.IsNullOrWhiteSpace(line))
                .Select(row => _transactionMapper.CreateTransaction(row, columnIndices));
        }

        private void LogColumnIndices(IDictionary<FidelityCsvColumn, int> headers)
        {
            var headerColumns = string.Join(",", headers.Select(kvp => kvp.Key + "=" + kvp.Value));
            _log.Debug($"Parsed CSV header=columns: {headerColumns}");
        }
    }
}