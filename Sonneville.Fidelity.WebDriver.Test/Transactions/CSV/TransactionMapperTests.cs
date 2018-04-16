using System;
using System.Collections.Generic;
using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.WebDriver.Transactions;
using Sonneville.Fidelity.WebDriver.Transactions.CSV;
using Sonneville.Investing.Domain;

namespace Sonneville.Fidelity.WebDriver.Test.Transactions.CSV
{
    [TestFixture]
    public class TransactionMapperTests
    {
        private Dictionary<FidelityCsvColumn, int> _columnMappings;

        private TransactionMapper _transactionMapper;
        private string _record;

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
            _record =
                " 12/18/2015,Account 1234, awesome profit, ticker, ticker description,Cash,1.2,3.4,,-7.8,9.01,234.56, ";

            var transactionTypeMapperMock = new Mock<ITransactionTypeMapper>();
            transactionTypeMapperMock.Setup(mapper => mapper.ClassifyDescription("awesome profit"))
                .Returns(TransactionType.Sell);

            _transactionMapper = new TransactionMapper(new Mock<ILog>().Object, transactionTypeMapperMock.Object);
        }

        [Test]
        public void ShouldMapBasedOnColumnMappings()
        {
            var actualTransaction = _transactionMapper.CreateTransaction(_record, _columnMappings);

            Assert.AreEqual(new DateTime(2015, 12, 18), actualTransaction.RunDate);
            Assert.AreEqual("Account 1234", actualTransaction.AccountNumber);
            Assert.AreEqual("awesome profit", actualTransaction.Action);
            Assert.AreEqual("ticker", actualTransaction.Symbol);
            Assert.AreEqual("ticker description", actualTransaction.SecurityDescription);
            Assert.AreEqual("Cash", actualTransaction.SecurityType);
            Assert.AreEqual(1.2m, actualTransaction.Quantity);
            Assert.AreEqual(3.4m, actualTransaction.Price);
            Assert.IsNull(actualTransaction.Commission);
            Assert.AreEqual(-7.8m, actualTransaction.Fees);
            Assert.AreEqual(9.01m, actualTransaction.AccruedInterest);
            Assert.AreEqual(234.56m, actualTransaction.Amount);
            Assert.IsNull(actualTransaction.SettlementDate);
            Assert.AreEqual(TransactionType.Sell, actualTransaction.Type);
        }

        [Test]
        public void ShouldReturnEmptyAccountIfColumnNotFound()
        {
            _columnMappings.Remove(FidelityCsvColumn.Account);

            var actualTransaction = _transactionMapper.CreateTransaction(_record, _columnMappings);

            Assert.AreEqual(new DateTime(2015, 12, 18), actualTransaction.RunDate);
            Assert.AreEqual(string.Empty, actualTransaction.AccountNumber);
            Assert.AreEqual("awesome profit", actualTransaction.Action);
            Assert.AreEqual("ticker", actualTransaction.Symbol);
            Assert.AreEqual("ticker description", actualTransaction.SecurityDescription);
            Assert.AreEqual("Cash", actualTransaction.SecurityType);
            Assert.AreEqual(1.2m, actualTransaction.Quantity);
            Assert.AreEqual(3.4m, actualTransaction.Price);
            Assert.IsNull(actualTransaction.Commission);
            Assert.AreEqual(-7.8m, actualTransaction.Fees);
            Assert.AreEqual(9.01m, actualTransaction.AccruedInterest);
            Assert.AreEqual(234.56m, actualTransaction.Amount);
            Assert.IsNull(actualTransaction.SettlementDate);
            Assert.AreEqual(TransactionType.Sell, actualTransaction.Type);
        }
    }
}