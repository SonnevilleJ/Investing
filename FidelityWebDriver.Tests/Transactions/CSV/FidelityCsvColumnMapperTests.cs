using NUnit.Framework;
using Sonneville.FidelityWebDriver.Transactions.CSV;

namespace Sonneville.FidelityWebDriver.Tests.Transactions.Csv
{
    [TestFixture]
    public class FidelityCsvColumnMapperTests
    {
        [Test]
        [TestCase(" Run Date ", FidelityCsvColumn.RunDate)]
        [TestCase(" Account ", FidelityCsvColumn.Account)]
        [TestCase(" Action ", FidelityCsvColumn.Action)]
        [TestCase(" Symbol ", FidelityCsvColumn.Symbol)]
        [TestCase(" Security Description ", FidelityCsvColumn.SecurityDescription)]
        [TestCase(" Security Type ", FidelityCsvColumn.SecurityType)]
        [TestCase(" Quantity ", FidelityCsvColumn.Quantity)]
        [TestCase(" Price ", FidelityCsvColumn.Price)]
        [TestCase(" Commission ", FidelityCsvColumn.Commission)]
        [TestCase(" Fees ", FidelityCsvColumn.Fees)]
        [TestCase(" Accrued Interest ", FidelityCsvColumn.AccruedInterest)]
        [TestCase(" Amount ", FidelityCsvColumn.Amount)]
        [TestCase(" Settlement Date ", FidelityCsvColumn.SettlementDate)]
        [TestCase(" gibberish ", FidelityCsvColumn.Unknown)]
        public void ShouldMapAndTrimValues(string value, FidelityCsvColumn expectedColumn)
        {
            var actualColumn = new FidelityCsvColumnMapper().GetHeader(value);

            Assert.AreEqual(expectedColumn, actualColumn);
        }

        [Test]
        public void ShouldMapMultipleColumns()
        {
            var headers = new FidelityCsvColumnMapper().GetColumnMappings(" Account ,  Settlement Date ,  Amount ");

            Assert.AreEqual(0, headers[FidelityCsvColumn.Account]);
            Assert.AreEqual(1, headers[FidelityCsvColumn.SettlementDate]);
            Assert.AreEqual(2, headers[FidelityCsvColumn.Amount]);
            Assert.AreEqual(3, headers.Count);
        }

        [Test]
        public void ShouldMapMultipleUnknownColumns()
        {
            var headers = new FidelityCsvColumnMapper().GetColumnMappings("asdf,xyz,blahblah");

            Assert.AreEqual(0, headers.Count);
        }

        [Test]
        public void ShouldMapCorrectIdsDespiteSkippedColumns()
        {
            var headers = new FidelityCsvColumnMapper().GetColumnMappings("asdf, Account ");

            Assert.AreEqual(1, headers[FidelityCsvColumn.Account]);
            Assert.AreEqual(1, headers.Count);
        }
    }
}