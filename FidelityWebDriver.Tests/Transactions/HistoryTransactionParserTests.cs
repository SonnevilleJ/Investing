using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Transactions;

namespace Sonneville.FidelityWebDriver.Tests.Transactions
{
    [TestFixture]
    public class HistoryTransactionParserTests
    {
        private readonly HistoryHtmlGenerator _historyHtmlGenerator = new HistoryHtmlGenerator();
        private readonly TransactionTypeMapper _transactionTypeMapper = new TransactionTypeMapper();

        private Mock<IWebElement> _historyRootDivMock;
        private Mock<IWebElement> _historyTableBodyMock;

        private HistoryTransactionParser _historyTransactionParser;

        [SetUp]
        public void Setup()
        {
            _historyTableBodyMock = new Mock<IWebElement>();

            _historyRootDivMock = new Mock<IWebElement>();
            _historyRootDivMock.Setup(div => div.FindElements(By.TagName("tbody")))
                .Returns(new List<IWebElement>
                {
                    _historyTableBodyMock.Object,
                    new Mock<IWebElement>().Object,
                }.AsReadOnly);

            _historyTransactionParser = new HistoryTransactionParser(_transactionTypeMapper);
        }

        [Test]
        public void GetHistoryShouldParseDepositTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateDepositTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseDepositBrokerageLinkTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateDepositBrokeragelinkTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseDepositHsaTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateDepositHsaTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseDividendReceiptTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateDividendReceivedTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseDividendReinvestmentTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateDividendReinvestmentTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseWithdrawalTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateWithdrawalTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseShortTermCapitalGainTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateShortTermCapitalGainTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseLongTermCapitalGainTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateLongTermCapitalGainTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseBuyTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateBuyTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseSellTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateSellTransaction()
            };
            SetupHistoryTable(expectedTransactions);

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        [Test]
        public void GetHistoryShouldParseInLieuOfSellTransactions()
        {
            var expectedTransactions = new List<FidelityTransaction>
            {
                CreateSellTransaction()
            };
            SetupHistoryTable(expectedTransactions, new[] {"Commission", "Settlement Date"});
            expectedTransactions.Single().Commission = null;
            expectedTransactions.Single().SettlementDate = null;

            var actualTransactions = _historyTransactionParser.ParseFidelityTransactions(_historyRootDivMock.Object);

            CollectionAssert.AreEquivalent(expectedTransactions, actualTransactions);
        }

        private void SetupHistoryTable(IEnumerable<FidelityTransaction> expectedTransactions, ICollection<string> excludedKeys = null)
        {
            _historyTableBodyMock.Setup(div => div.FindElements(By.TagName("tr")))
                .Returns(expectedTransactions
                    .SelectMany(transaction => _historyHtmlGenerator.MapToTableRows(transaction, excludedKeys))
                    .ToList()
                    .AsReadOnly());
        }

        private FidelityTransaction CreateDepositTransaction(TransactionType transactionType = TransactionType.Deposit)
        {
            return new FidelityTransaction
            {
                Type = transactionType,
                RunDate = new DateTime(2016, 12, 26),
                AccountName = "account name",
                AccountNumber = "account number",
                SecurityDescription = _transactionTypeMapper.GetSampleDescription(transactionType),
                Amount = 1234.50m,
            };
        }

        private FidelityTransaction CreateDepositBrokeragelinkTransaction()
        {
            return CreateDepositTransaction(TransactionType.DepositBrokeragelink);
        }

        private FidelityTransaction CreateDepositHsaTransaction()
        {
            return CreateDepositTransaction(TransactionType.DepositHSA);
        }

        private FidelityTransaction CreateWithdrawalTransaction()
        {
            return CreateDepositTransaction(TransactionType.Withdrawal);
        }

        private FidelityTransaction CreateDividendReceivedTransaction()
        {
            const TransactionType transactionType = TransactionType.DividendReceipt;
            return new FidelityTransaction
            {
                Type = transactionType,
                RunDate = new DateTime(2016, 12, 26),
                AccountName = "account name",
                AccountNumber = "account number",
                Symbol = "ASDF",
                SecurityDescription = _transactionTypeMapper.GetSampleDescription(transactionType),
                Amount = 1234.50m,
            };
        }

        private FidelityTransaction CreateDividendReinvestmentTransaction()
        {
            const TransactionType transactionType = TransactionType.DividendReinvestment;
            return new FidelityTransaction
            {
                Type = transactionType,
                RunDate = new DateTime(2016, 12, 26),
                AccountName = "account name",
                AccountNumber = "account number",
                Symbol = "ASDF",
                SecurityDescription = _transactionTypeMapper.GetSampleDescription(transactionType),
                Quantity = 0.012m,
                Price = 1.23m,
                Amount = 1234.50m,
            };
        }

        private FidelityTransaction CreateShortTermCapitalGainTransaction(TransactionType transactionType = TransactionType.ShortTermCapGain)
        {
            return new FidelityTransaction
            {
                Type = transactionType,
                RunDate = new DateTime(2016, 12, 26),
                AccountName = "account name",
                AccountNumber = "account number",
                Symbol = "ASDF",
                SecurityDescription = _transactionTypeMapper.GetSampleDescription(transactionType),
                Amount = 1234.50m,
            };
        }

        private FidelityTransaction CreateLongTermCapitalGainTransaction()
        {
            return CreateShortTermCapitalGainTransaction(TransactionType.LongTermCapGain);
        }

        private FidelityTransaction CreateBuyTransaction()
        {
            const TransactionType transactionType = TransactionType.Buy;
            return new FidelityTransaction
            {
                Type = transactionType,
                RunDate = new DateTime(2016, 12, 26),
                AccountName = "account name",
                AccountNumber = "account number",
                Symbol = "ASDF",
                SecurityDescription = _transactionTypeMapper.GetSampleDescription(transactionType),
                Quantity = 0.012m,
                Price = 1.23m,
                Amount = 1234.50m,
                SettlementDate = new DateTime(2016, 12, 31),
            };
        }

        private FidelityTransaction CreateSellTransaction()
        {
            const TransactionType transactionType = TransactionType.Sell;
            return new FidelityTransaction
            {
                Type = transactionType,
                RunDate = new DateTime(2016, 12, 26),
                AccountName = "account name",
                AccountNumber = "account number",
                Symbol = "ASDF",
                SecurityDescription = _transactionTypeMapper.GetSampleDescription(transactionType),
                Quantity = -0.012m,
                Price = 1.23m,
                Amount = 1234.50m,
                Commission = 7.95m,
                SettlementDate = new DateTime(2016, 12, 31),
            };
        }
    }
}
