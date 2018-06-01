using NUnit.Framework;
using Sonneville.Utilities.Security;

namespace Sonneville.Utilities.Test.Security
{
    [TestFixture]
    public class FingerprintManagerTests
    {
        private FingerprintManager _fingerprintManager;
        private SaltGenerator _saltGenerator;

        [SetUp]
        public void Setup()
        {
            var hashAlgorithm = HashAlgorithm.SHA512;
            var iterations = 100;
            _saltGenerator = new SaltGenerator();
            _fingerprintManager = new FingerprintManager(new Pbkdf2SaltedCryptor(hashAlgorithm, iterations), hashAlgorithm, iterations, new SaltGenerator());
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
