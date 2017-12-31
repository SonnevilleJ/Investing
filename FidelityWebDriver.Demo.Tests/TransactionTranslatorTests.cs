using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;

namespace Sonneville.FidelityWebDriver.Demo.Tests
{
    [TestFixture]
    public class TransactionTranslatorTests
    {
        [Test]
        [TestCase(TransactionType.Deposit, "deposited")]
        [TestCase(TransactionType.Withdrawal, "withdrew")]
        [TestCase(TransactionType.Buy, "bought")]
        [TestCase(TransactionType.Sell, "sold")]
        [TestCase(TransactionType.BuyToCover, "bought to cover short position")]
        [TestCase(TransactionType.SellShort, "sold short")]
        [TestCase(TransactionType.DividendReceipt, "received")]
        [TestCase(TransactionType.DividendReinvestment, "reinvested")]
        [TestCase(TransactionType.Unknown, "Unknown transaction type: Unknown")]
        public void ShouldTranslateTransactionType(TransactionType type, string expectedTranslation)
        {
            var actualTranslation = new TransactionTranslator().Translate(type);

            Assert.AreEqual(expectedTranslation, actualTranslation);
        }
    }
}