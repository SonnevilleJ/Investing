using System;
using System.Text;
using NUnit.Framework;
using Sonneville.Utilities.Security;

namespace Sonneville.Utilities.Test.Security
{
    [TestFixture]
    public class Pbkdf2CryptorTests
    {
        private ISaltGenerator _saltGenerator;

        [SetUp]
        public void Setup()
        {
            _saltGenerator = new SaltGenerator();
        }

        [Test]
        [TestCase("one", "two", 8, "SHA512", 1, 512)]
        [TestCase("one", "One", 8, "SHA512", 1, 512)]
        public void DifferentDataShouldProduceDifferentDigest(
            string message1, string message2, int saltLength, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations);

            var data1 = StringToBytes(message1);
            var data2 = StringToBytes(message2);
            var salt = _saltGenerator.GenerateSalt(saltLength);
            var digest1 = cryptor.HashBytes(data1, salt);
            var digest2 = cryptor.HashBytes(data2, salt);

            Assert.AreNotEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 0, "SHA512", 1, 512)]
        [TestCase("secret", 7, "SHA512", 1, 512)]
        public void InvalidSaltLengthShouldThrow(
            string message, int saltBytes, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations);

            var data = StringToBytes(message);
            var salt = _saltGenerator.GenerateSalt(saltBytes);

            Assert.Throws<ArgumentException>(() =>
                cryptor.HashBytes(data, salt));
        }

        [Test]
        [TestCase("secret", 8, 16, "SHA512", 1, 512)]
        public void DifferentSaltLengthsShouldProduceDifferentDigest(
            string message, int salt1Length, int salt2Length, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations);

            var data = StringToBytes(message);
            var salt1 = new byte[salt1Length];
            var salt2 = new byte[salt2Length];
            var digest1 = cryptor.HashBytes(data, salt1);
            var digest2 = cryptor.HashBytes(data, salt2);

            Assert.AreNotEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 8, "SHA512", 1, 512)]
        [TestCase("secret", 512, "SHA512", 1, 512)]
        public void DifferentSaltsShouldProduceDifferentDigest(
            string message, int saltBytes, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations);

            var data = StringToBytes(message);
            var salt1 = _saltGenerator.GenerateSalt(saltBytes);
            var salt2 = _saltGenerator.GenerateSalt(saltBytes);
            var digest1 = cryptor.HashBytes(data, salt1);
            var digest2 = cryptor.HashBytes(data, salt2);

            Assert.AreNotEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 8, "SHA1", "SHA256", 1, 256)]
        [TestCase("secret", 8, "SHA1", "SHA384", 1, 384)]
        [TestCase("secret", 8, "SHA1", "SHA512", 1, 512)]
        [TestCase("secret", 8, "SHA256", "SHA512", 1, 512)]
        [TestCase("secret", 8, "SHA384", "SHA512", 1, 512)]
        public void DifferentalgorithmsShouldProduceDifferentDigest(
            string message, int saltBytes, string algorithm1, string algorithm2, int iterations, int digestLength
        )
        {
            var cryptor1 = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm1), iterations);
            var cryptor2 = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm2), iterations);

            var data = StringToBytes(message);
            var salt = _saltGenerator.GenerateSalt(saltBytes);
            var digest1 = cryptor1.HashBytes(data, salt);
            var digest2 = cryptor2.HashBytes(data, salt);

            Assert.AreNotEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 8, "SHA512", -1, 512)]
        [TestCase("secret", 8, "SHA512", 0, 512)]
        [TestCase("secret", 8, "SHA512", int.MinValue, 512)]
        public void InvalidIterationsShouldThrow(
            string message, int saltBytes, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations);

            var data = StringToBytes(message);
            var salt = _saltGenerator.GenerateSalt(saltBytes);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                cryptor.HashBytes(data, salt));
        }

        [Test]
        [TestCase("secret", 8, "SHA512", 1, 10, 512)]
        [TestCase("secret", 8, "SHA512", 2, 10, 512)]
        [TestCase("secret", 8, "SHA512", 3, 10, 512)]
        [TestCase("secret", 8, "SHA512", 4, 10, 512)]
        [TestCase("secret", 8, "SHA512", 5, 10, 512)]
        public void DifferentIterationsShouldProduceDifferentDigest(
            string message, int saltBytes, string algorithm, int iterations1, int iterations2, int digestLength
        )
        {
            var cryptor1 = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations1);
            var cryptor2 = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations2);

            var data = StringToBytes(message);
            var salt = _saltGenerator.GenerateSalt(saltBytes);
            var digest1 = cryptor1.HashBytes(data, salt);
            var digest2 = cryptor2.HashBytes(data, salt);

            Assert.AreNotEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 8, "SHA512", 1, 512)]
        public void SameSettingsShouldProduceSameDigest(
            string message, int saltLength, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations);

            var data = StringToBytes(message);
            var salt = _saltGenerator.GenerateSalt(saltLength);
            var digest1 = cryptor.HashBytes(data, salt);
            var digest2 = cryptor.HashBytes(data, salt);

            Assert.AreEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 8, "SHA512", 1, 512)]
        public void ShouldConvertTextFromUnicode(
            string text, int saltBytes, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations);

            var salt = _saltGenerator.GenerateSalt(saltBytes);
            var digestText = cryptor.HashString(text, salt);
            var digestBytes = cryptor.HashBytes(StringToBytes(text), salt);

            Assert.AreEqual(digestBytes, digestText);
        }

        private static byte[] StringToBytes(string message)
        {
            return Encoding.Unicode.GetBytes(message);
        }
    }
}
