using System;
using NUnit.Framework;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.Transactions;

namespace Sonneville.Investing.Fidelity.Test.Transactions
{
    [TestFixture]
    public class TransactionTypeMapperTests
    {
        private TransactionTypeMapper _transactionTypeMapper;

        [SetUp]
        public void Setup()
        {
            _transactionTypeMapper = new TransactionTypeMapper();
        }

        [Test]
        [TestCase("DIVIDEND RECEIVED", TransactionType.DividendReceipt)]
        [TestCase("REINVESTMENT", TransactionType.DividendReinvestment)]
        [TestCase("YOU SOLD             EXCHANGE", TransactionType.Sell)]
        [TestCase("IN LIEU OF FRX SHARE", TransactionType.Sell)]
        [TestCase("YOU BOUGHT           PROSPECTUS UNDER    SEPARATE COVER", TransactionType.Buy)]
        [TestCase("SHORT-TERM CAP GAIN", TransactionType.ShortTermCapGain)]
        [TestCase("LONG-TERM CAP GAIN", TransactionType.LongTermCapGain)]
        [TestCase("Electronic Funds Transfer Received", TransactionType.Deposit)]
        [TestCase("CASH CONTRIBUTION CURRENT YEAR", TransactionType.Deposit)]
        [TestCase("DIRECT DEPOSIT ELAN CARDSVCRedemption (Cash)", TransactionType.Deposit)]
        [TestCase("TRANSFERRED FROM     TO BROKERAGE OPTION", TransactionType.DepositBrokeragelink)]
        [TestCase("PARTIC CONTR CURRENT PARTICIPANT CUR YR", TransactionType.DepositHSA)]
        [TestCase("INTEREST EARNED", TransactionType.InterestEarned)]
        [TestCase("abcdefghijklmnopqrstuvwxyz", TransactionType.Unknown)]
        public void ShouldMapValues(string value, TransactionType expectedType)
        {
            var actualType = _transactionTypeMapper.ClassifyDescription(value);

            Assert.AreEqual(expectedType, actualType);
        }

        [Test]
        [TestCase(TransactionType.Deposit)]
        [TestCase(TransactionType.DepositBrokeragelink)]
        [TestCase(TransactionType.DepositHSA)]
        [TestCase(TransactionType.Withdrawal)]
        [TestCase(TransactionType.Buy)]
        [TestCase(TransactionType.Sell)]
        [TestCase(TransactionType.InterestEarned)]
        [TestCase(TransactionType.DividendReceipt)]
        [TestCase(TransactionType.ShortTermCapGain)]
        [TestCase(TransactionType.LongTermCapGain)]
        [TestCase(TransactionType.DividendReinvestment)]
        public void ShouldRoundripExampleDescription(TransactionType transactionType)
        {
            var exampleDescription = _transactionTypeMapper.GetExampleDescription(transactionType);
            var returnedTransactionType = _transactionTypeMapper.ClassifyDescription(exampleDescription);
            Assert.AreEqual(transactionType, returnedTransactionType);
        }

        /// <summary>
        /// This test covers the transactions I have not yet seen.
        /// I don't know what descriptions Fidelity gives them.
        /// </summary>
        [Test]
        [TestCase(TransactionType.SellShort)]
        [TestCase(TransactionType.BuyToCover)]
        [TestCase(TransactionType.Unknown)]
        public void ShouldThrowForNewTransactionTypes(TransactionType transactionType)
        {
            Assert.Throws<NotImplementedException>(() => _transactionTypeMapper.GetExampleDescription(transactionType));
        }
    }
}