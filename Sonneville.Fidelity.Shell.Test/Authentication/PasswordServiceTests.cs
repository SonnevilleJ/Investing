using NUnit.Framework;
using Sonneville.Fidelity.Shell.Authentication;
using Sonneville.Utilities.Security;

namespace Sonneville.Fidelity.Shell.Test.Authentication
{
    [TestFixture]
    public class PasswordServiceTests
    {
        private HashAlgorithm _hashAlgorithm;
        private int _iterations;
        private PasswordService _passwordService;
        private Pbkdf2SaltedCryptor _saltedCryptor;

        [SetUp]
        public void Setup()
        {
            _hashAlgorithm = HashAlgorithm.SHA512;

            _iterations = 1000;

            _saltedCryptor = new Pbkdf2SaltedCryptor(_hashAlgorithm, _iterations);

            _passwordService = new PasswordService(new SaltGenerator(), _saltedCryptor);
        }

        [Test]
        public void ShouldGeneratePasswordHash()
        {
            var passwordHash = _passwordService.HashPassword("password");

            Assert.AreEqual(_saltedCryptor.Name, passwordHash.CryptorName);
            Assert.AreEqual(_saltedCryptor.Algorithm, passwordHash.HashAlgorithm);
            Assert.AreEqual(_saltedCryptor.Iterations, passwordHash.Iterations);
            Assert.NotNull(passwordHash.Salt);
            Assert.NotNull(passwordHash.HashDigest);
        }

        [Test]
        [TestCase("password")]
        public void ShouldValidatePasswordWhenPasswordMatches(string password)
        {
            var passwordHash = _passwordService.HashPassword(password);

            Assert.IsTrue(_passwordService.ValidatePassword(password, passwordHash));
        }

        [Test]
        [TestCase("password", "pa55w0rd")]
        [TestCase("password", "password ")]
        public void ShouldNotValidatePasswordWhenPasswordDoesNotMatch(string originalPassword, string enteredPassword)
        {
            var passwordHash = _passwordService.HashPassword(originalPassword);

            Assert.IsFalse(_passwordService.ValidatePassword(enteredPassword, passwordHash));
        }
    }
}