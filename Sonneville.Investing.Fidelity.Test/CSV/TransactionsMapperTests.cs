using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.CSV;

namespace Sonneville.Investing.Fidelity.Test.CSV
{
    [TestFixture]
    public class TransactionsMapperTests
    {
        private string _csvHeaders;
        private string[] _csvRecords;
        private Dictionary<FidelityCsvColumn, int> _columnMappings;
        private Mock<ILog> _logMock;
        private Mock<IFidelityCsvColumnMapper> _columnMapperMock;
        private Mock<ITransactionMapper> _transactionMapperMock;
        private TransactionsMapper _transactionsMapper;

        [SetUp]
        public void Setup()
        {
            _csvHeaders = $"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}csv headers";
            _csvRecords = new[]
            {
                "record 1 col a, col b, col c",
                "record 2 col a, col b, col c",
                "",
                "garbage - this, should be, filtered, out"
            };

            _columnMappings = new Dictionary<FidelityCsvColumn, int>
            {
                {FidelityCsvColumn.Symbol, 123},
                {FidelityCsvColumn.Commission, 456},
            };

            _columnMapperMock = new Mock<IFidelityCsvColumnMapper>();
            _columnMapperMock.Setup(mapper => mapper.GetColumnMappings(_csvHeaders.Trim())).Returns(_columnMappings);

            _transactionMapperMock = new Mock<ITransactionMapper>();

            _logMock = new Mock<ILog>();

            _transactionsMapper =
                new TransactionsMapper(_logMock.Object, _columnMapperMock.Object, _transactionMapperMock.Object);
        }

        [Test]
        [TestCase("\n")]
        [TestCase("\r\n")]
        public void ShouldMapColumnsAndPassEachRecordToTransactionMapper(string newLine)
        {
            var expectedTransactions = SetupExpectedTransactions(_csvRecords);
            var csvContent = SetupCsv(_csvHeaders, newLine, _csvRecords);

            var actualTransactions = _transactionsMapper.ParseCsv(csvContent);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void ShouldLogColumnMappings()
        {
            var csvContent = SetupCsv(_csvHeaders, Environment.NewLine, _csvRecords);

            _transactionsMapper.ParseCsv(csvContent);

            _columnMappings.Select(mapping => $"{mapping.Key}={mapping.Value}")
                .ToList()
                .ForEach(columnMapping =>
                    _logMock.Verify(log => log.Debug(It.Is<string>(message => message.Contains($"{columnMapping}"))))
                );
        }

        private static string SetupCsv(string headerRow, string newLine, params string[] records)
        {
            return $"{headerRow}{newLine}{string.Join(newLine, records)}";
        }

        private IEnumerable<ITransaction> SetupExpectedTransactions(IEnumerable<string> csvRecords)
        {
            return csvRecords.TakeWhile(record => !string.IsNullOrWhiteSpace(record))
                .Select(SetupMockTransaction)
                .ToList();
        }

        private ITransaction SetupMockTransaction(string record)
        {
            var transactionMock = new Mock<ITransaction>();
            _transactionMapperMock.Setup(mapper => mapper.CreateTransaction(record, _columnMappings))
                .Returns(transactionMock.Object);
            return transactionMock.Object;
        }
    }
}