using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Transactions;
using Sonneville.FidelityWebDriver.Transactions.CSV;

namespace Sonneville.FidelityWebDriver.Tests.Transactions.Csv
{
    [TestFixture]
    public class TransactionMapperTests
    {
        private Dictionary<FidelityCsvColumn, int> _columnMappings;

        private TransactionMapper _transactionMapper;

        [SetUp]
        public void Setup()
        {
            _columnMappings = new Dictionary<FidelityCsvColumn, int>
            {
                {FidelityCsvColumn.RunDate, 0},
                {FidelityCsvColumn.Account, 1},
                {FidelityCsvColumn.Action, 2},
                {FidelityCsvColumn.Symbol, 3},
                {FidelityCsvColumn.SecurityDescription, 4},
                {FidelityCsvColumn.SecurityType, 5},
                {FidelityCsvColumn.Quantity, 6},
                {FidelityCsvColumn.Price, 7},
                {FidelityCsvColumn.Commission, 8},
                {FidelityCsvColumn.Fees, 9},
                {FidelityCsvColumn.AccruedInterest, 10},
                {FidelityCsvColumn.Amount, 11},
                {FidelityCsvColumn.SettlementDate, 12},
            };

            var transactionTypeMapperMock = new Mock<ITransactionTypeMapper>();
            transactionTypeMapperMock.Setup(mapper => mapper.ClassifyDescription("awesome profit"))
                .Returns(TransactionType.Sell);

            _transactionMapper = new TransactionMapper(transactionTypeMapperMock.Object);
        }

        [Test]
        public void ShouldMapBasedOnColumnMappings()
        {
            var record =
                " 12/18/2015,Account 1234, awesome profit, ticker, ticker description,Cash,1.2,3.4,,-7.8,9.01,234.56, ";

            var actualTransaction = _transactionMapper.CreateTransaction(record, _columnMappings);

            var first = actualTransaction;
            Assert.AreEqual(new DateTime(2015, 12, 18), first.RunDate);
            Assert.AreEqual("Account 1234", first.AccountNumber);
            Assert.AreEqual("awesome profit", first.Action);
            Assert.AreEqual("ticker", first.Symbol);
            Assert.AreEqual("ticker description", first.SecurityDescription);
            Assert.AreEqual("Cash", first.SecurityType);
            Assert.AreEqual(1.2m, first.Quantity);
            Assert.AreEqual(3.4m, first.Price);
            Assert.IsNull(first.Commission);
            Assert.AreEqual(-7.8m, first.Fees);
            Assert.AreEqual(9.01m, first.AccruedInterest);
            Assert.AreEqual(234.56m, first.Amount);
            Assert.IsNull(first.SettlementDate);
            Assert.AreEqual(TransactionType.Sell, first.Type);
        }
    }
}