using System;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;

namespace Sonneville.FidelityWebDriver.Tests.Data
{
    [TestFixture]
    public class FidelityTransactionTests
    {
        [Test]
        public void ShouldEqualIdenticalTransaction()
        {
            var one = new FidelityTransaction
            {
                AccountName = "account name",
                AccountNumber = "account number",
                AccruedInterest = 1234.2m,
                Action = "action",
                Amount = 2345.6m,
                Commission = 3456.7m,
                Fees = 45.6m,
                Price = 56.7m,
                Quantity = 67.8m,
                RunDate = DateTime.Now,
                SecurityDescription = "security description",
                SecurityType = "security type",
                SettlementDate = DateTime.Now.AddMilliseconds(235),
                Symbol = "symbol",
                Type = TransactionType.Buy
            };
            var two = CopyTransaction(one);

            Assert.AreEqual(one, two);
        }

        [Test]
        public void ShouldCreateStringRepresentation()
        {
            var one = new FidelityTransaction
            {
                AccountName = "account name",
                AccountNumber = "account number",
                AccruedInterest = 1234.2m,
                Action = "action",
                Amount = 2345.6m,
                Commission = 3456.7m,
                Fees = 45.6m,
                Price = 56.7m,
                Quantity = 67.8m,
                RunDate = DateTime.Now,
                SecurityDescription = "security description",
                SecurityType = "security type",
                SettlementDate = DateTime.Now.AddMilliseconds(235),
                Symbol = "symbol",
                Type = TransactionType.Buy
            };

            Assert.False(string.IsNullOrEmpty(one.ToString()));
        }

        private static FidelityTransaction CopyTransaction(FidelityTransaction source)
        {
            return new FidelityTransaction
            {
                AccountName = source.AccountName,
                AccountNumber = source.AccountNumber,
                AccruedInterest = source.AccruedInterest,
                Action = source.Action,
                Amount = source.Amount,
                Commission = source.Commission,
                Fees = source.Fees,
                Price = source.Price,
                Quantity = source.Quantity,
                RunDate = source.RunDate,
                SecurityDescription = source.SecurityDescription,
                SecurityType = source.SecurityType,
                SettlementDate = source.SettlementDate,
                Symbol = source.Symbol,
                Type = source.Type
            };
        }
    }
}