using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Transactions;

namespace Sonneville.FidelityWebDriver.Tests.Transactions
{
    [TestFixture]
    public class TransactionTypeMapperTests
    {
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
            var actualType = new TransactionTypeMapper().ClassifyDescription(value);

            Assert.AreEqual(expectedType, actualType);
        }

        [Test]
        [TestCase(TransactionType.DividendReceipt, "DIVIDEND RECEIVED")]
        [TestCase(TransactionType.DividendReinvestment, "REINVESTMENT")]
        [TestCase(TransactionType.Sell, "YOU SOLD             EXCHANGE")]
        [TestCase(TransactionType.Buy, "YOU BOUGHT           PROSPECTUS UNDER    SEPARATE COVER")]
        [TestCase(TransactionType.ShortTermCapGain, "SHORT-TERM CAP GAIN")]
        [TestCase(TransactionType.LongTermCapGain, "LONG-TERM CAP GAIN")]
        [TestCase(TransactionType.Deposit, "Electronic Funds Transfer Received")]
        [TestCase(TransactionType.DepositBrokeragelink, "TRANSFERRED FROM     TO BROKERAGE OPTION")]
        [TestCase(TransactionType.DepositHSA, "PARTIC CONTR CURRENT PARTICIPANT CUR YR")]
        public void ShouldMapKeys(TransactionType key, string expectedValue)
        {
            var actualValue = new TransactionTypeMapper().GetSampleDescription(key);

            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ShouldNotMapUnknownKey()
        {
            Assert.Throws<KeyNotFoundException>(
                () => new TransactionTypeMapper().GetSampleDescription(TransactionType.Unknown));
        }
    }
}