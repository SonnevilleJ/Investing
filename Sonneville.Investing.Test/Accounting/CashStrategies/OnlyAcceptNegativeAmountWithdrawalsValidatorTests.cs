using System;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Cash.Transactions;
using Sonneville.Investing.Accounting.CashStrategies;

namespace Sonneville.Investing.Test.Accounting.CashStrategies
{
    [TestFixture]
    public class OnlyAcceptNegativeAmountWithdrawalsValidatorTests
    {
        [SetUp]
        public void Setup()
        {
            _validator = new OnlyAcceptNegativeAmountWithdrawalsStrategy();
        }

        private OnlyAcceptNegativeAmountWithdrawalsStrategy _validator;

        [Test]
        public void ShouldNotThrowForNegativeAmounts()
        {
            var withdrawal = new Withdrawal(DateTime.Today, 1.0m);
            _validator.ThrowIfInvalid(withdrawal, null);
        }

        [Test]
        public void ShouldThrowForNullWithdrawals()
        {
            Assert.Throws<ArgumentNullException>(() => _validator.ThrowIfInvalid(null, null));
        }

        [Test]
        public void ShouldThrowForPositiveAmounts()
        {
            var withdrawal = new Withdrawal(DateTime.Today, -1.0m);
            Assert.Throws<InvalidOperationException>(() => _validator.ThrowIfInvalid(withdrawal, null));
        }

        [Test]
        public void ShouldThrowForZeroAmounts()
        {
            var withdrawal = new Withdrawal(DateTime.Today, 0m);
            Assert.Throws<InvalidOperationException>(() => _validator.ThrowIfInvalid(withdrawal, null));
        }
    }
}