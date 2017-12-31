using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Transactions.CSV;

namespace Sonneville.FidelityWebDriver.Tests.Transactions.Csv
{
    [TestFixture]
    public class TransactionsMapperTests
    {
        private TransactionsMapper _transactionsMapper;
        private Mock<IFidelityCsvColumnMapper> _columnMapperMock;
        private Dictionary<FidelityCsvColumn, int> _columnMappings;
        private Mock<ITransactionMapper> _transactionMapperMock;
        private string _headerRow;

        [SetUp]
        public void Setup()
        {
            _headerRow = "\n\n\ncsv headers";
            _columnMappings = new Dictionary<FidelityCsvColumn, int>();

            _columnMapperMock = new Mock<IFidelityCsvColumnMapper>();
            _columnMapperMock.Setup(mapper => mapper.GetColumnMappings(_headerRow.Trim())).Returns(_columnMappings);

            _transactionMapperMock = new Mock<ITransactionMapper>();

            _transactionsMapper = new TransactionsMapper(_columnMapperMock.Object, _transactionMapperMock.Object);
        }

        [Test]
        public void ShouldMapColumnsAndPassEachRecordToTransactionMapper()
        {
            var records = new[]
            {
                "record 1 col a, col b, col c",
                "record 2 col a, col b, col c",
                "",
                "garbage - this, should be, filtered, out"
            };
            var expectedTransactions = SetupExpectedTransactions(records).ToList();
            var csvContent = SetupCsv(_headerRow, "\n", records);

            var actualTransactions = _transactionsMapper.ParseCsv(csvContent);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        private IEnumerable<IFidelityTransaction> SetupExpectedTransactions(IEnumerable<string> records)
        {
            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record)) yield break;

                var transactionMock = new Mock<IFidelityTransaction>();
                _transactionMapperMock.Setup(mapper => mapper.CreateTransaction(record, _columnMappings))
                    .Returns(transactionMock.Object);
                yield return transactionMock.Object;
            }
        }

        private static string SetupCsv(string headerRow, string newLine, params string[] records)
        {
            return $"{headerRow}{newLine}{string.Join(newLine, records)}";
        }
    }
}