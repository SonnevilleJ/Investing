using System;
using NUnit.Framework;
using Sonneville.Investing.Accounting.Transactions;
using Sonneville.Investing.Accounting.Validation;

namespace Sonneville.Investing.Test.Accounting.Validation
{
    [TestFixture]
    public class OnlyAcceptPositiveOrZeroAmountDepositsValidatorTests
    {
        private OnlyAcceptPositiveOrZeroAmountDepositsValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new OnlyAcceptPositiveOrZeroAmountDepositsValidator();
        }

        [Test]
        public void ShouldNotThrowForPositiveAmounts()
        {
            var deposit = new Deposit(DateTime.Today, 1.0m);
            _validator.ThrowIfInvalid(deposit, null);
        }

        [Test]
        public void ShouldNotThrowForZeroAmounts()
        {
            var deposit = new Deposit(DateTime.Today, 0m);
            _validator.ThrowIfInvalid(deposit, null);
        }

        [Test]
        public void ShouldThrowForNegativeAmounts()
        {
            var deposit = new Deposit(DateTime.Today, -1.0m);
            Assert.Throws<InvalidOperationException>(() => _validator.ThrowIfInvalid(deposit, null));
        }

        [Test]
        public void ShouldThrowForNullDeposits()
        {
            Assert.Throws<ArgumentNullException>(() => _validator.ThrowIfInvalid(null, null));
        }
    }
}