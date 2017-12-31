using System;
using System.Collections.Generic;
using System.Linq;
using Sonneville.FidelityWebDriver.Data;

namespace Sonneville.FidelityWebDriver.Transactions.CSV
{
    public interface ITransactionsMapper
    {
        IList<IFidelityTransaction> ParseCsv(string csvContent);
    }

    public class TransactionsMapper : ITransactionsMapper
    {
        private readonly IFidelityCsvColumnMapper _fidelityCsvColumnMapper;
        private readonly ITransactionMapper _transactionMapper;

        public TransactionsMapper(IFidelityCsvColumnMapper fidelityCsvColumnMapper, ITransactionMapper transactionMapper)
        {
            _fidelityCsvColumnMapper = fidelityCsvColumnMapper;
            _transactionMapper = transactionMapper;
        }

        public IList<IFidelityTransaction> ParseCsv(string csvContent)
        {
            var rows = csvContent.Trim().Split(new[] {"\n"}, StringSplitOptions.None);
            var headers = _fidelityCsvColumnMapper.GetColumnMappings(rows.Take(1).Single());
            return rows.Skip(1)
                .TakeWhile(line => !string.IsNullOrWhiteSpace(line))
                .Select(row => _transactionMapper.CreateTransaction(row, headers))
                .ToList();
        }
    }
}