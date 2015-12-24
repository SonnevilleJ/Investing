using System;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Transactions;
using Sonneville.Investing.Accounting.Validation;

namespace Sonneville.Investing.Test.Accounting.Validation
{
    [TestFixture]
    public class OnlyAcceptNegativeAmountWithdrawalsValidatorTests
    {
        private OnlyAcceptNegativeAmountWithdrawalsValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new OnlyAcceptNegativeAmountWithdrawalsValidator();
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
    }
}