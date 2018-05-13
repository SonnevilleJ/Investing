using NUnit.Framework;
using Sonneville.Utilities.Security;

namespace Sonneville.Utilities.Test.Security
{
    [TestFixture]
    public class FingerprintManagerTests
    {
        private FingerprintManager _fingerprintManager;

        [SetUp]
        public void Setup()
        {
            _fingerprintManager = new FingerprintManager(new Pbkdf2Cryptor(), HashAlgorithm.SHA512, 100);
        }

        [Test]
        [TestCase("password")]
        public void ShouldValidateSuccessfullyFromSamePassword(string password)
        {
            var fingerprint = _fingerprintManager.HashPassword(password);

            Assert.IsTrue(_fingerprintManager.VerifyPassword(password, fingerprint));
        }

        [Test]
        [TestCase("one", "two")]
        [TestCase("one", "One")]
        public void ShouldValidateUnsuccessfullyFromDifferentPasswords(string password1, string password2)
        {
            var fingerprint = _fingerprintManager.HashPassword(password1);

            Assert.IsFalse(_fingerprintManager.VerifyPassword(password2, fingerprint));
        }

        [Test]
        [TestCase("password")]
        public void ShouldValidateUnsuccessfullyFromDifferentSalt(string password)
        {
            var fingerprint = _fingerprintManager.HashPassword(password);

            fingerprint.Salt[0]++;

            Assert.IsFalse(_fingerprintManager.VerifyPassword(password, fingerprint));
        }

        [Test]
        [TestCase("password")]
        public void ShouldValidateUnsuccessfullyFromDifferentAlgorithm(string password)
        {
            var fingerprint = _fingerprintManager.HashPassword(password);

            fingerprint.Algorithm = HashAlgorithm.SHA1.Name;

            Assert.IsFalse(_fingerprintManager.VerifyPassword(password, fingerprint));
        }

        [Test]
        [TestCase("password")]
        public void ShouldValidateUnsuccessfullyFromDifferentIterations(string password)
        {
            var fingerprint = _fingerprintManager.HashPassword(password);

            fingerprint.Iterations++;

            Assert.IsFalse(_fingerprintManager.VerifyPassword(password, fingerprint));
        }
    }
}
