﻿using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Transactions.CSV;

namespace Sonneville.FidelityWebDriver.Tests.Transactions.Csv
{
    [TestFixture]
    public class TransactionTypeMapperTests
    {
        [Test]
        [TestCase("DIVIDEND RECEIVED", TransactionType.DividendReceipt)]
        [TestCase("REINVESTMENT", TransactionType.DividendReinvestment)]
        [TestCase("YOU SOLD             EXCHANGE", TransactionType.Sell)]
        [TestCase("YOU BOUGHT           PROSPECTUS UNDER    SEPARATE COVER", TransactionType.Buy)]
        [TestCase("SHORT-TERM CAP GAIN", TransactionType.ShortTermCapGain)]
        [TestCase("LONG-TERM CAP GAIN", TransactionType.LongTermCapGain)]
        [TestCase("Electronic Funds Transfer Received", TransactionType.Deposit)]
        [TestCase("TRANSFERRED FROM     TO BROKERAGE OPTION", TransactionType.DepositBrokeragelink)]
        [TestCase("PARTIC CONTR CURRENT PARTICIPANT CUR YR", TransactionType.DepositHSA)]
        [TestCase("abcdefghijklmnopqrstuvwxyz", TransactionType.Unknown)]
        public void ShouldMapValues(string value, TransactionType expectedType)
        {
            var actualType = new TransactionTypeMapper().MapValue(value);

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
            var actualValue = new TransactionTypeMapper().MapKey(key);

            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ShouldNotMapUnknownKey()
        {
            Assert.Throws<KeyNotFoundException>(
                () => new TransactionTypeMapper().MapKey(TransactionType.Unknown));
        }
    }
}