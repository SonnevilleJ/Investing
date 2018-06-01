using System;
using System.Collections.Generic;
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
        [TestCase(1000000)]
        public void ShouldGenerateRandomSalt(int iterations)
        {
            var hashAlgorithm = HashAlgorithm.SHA512;
            var cryptor = new Pbkdf2SaltedCryptor(hashAlgorithm, iterations, _saltGenerator);

            var salts = new HashSet<byte[]>();
            for (var i = 0; i < iterations; i++)
            {
                if (!salts.Add(cryptor.GenerateSalt(BitsToBytes(hashAlgorithm.Length))))
                {
                    // Due to salt generation being random, duplicates are extremely rare but aren't impossible
                    Assert.Fail($"Duplicate detected after {salts.Count} {hashAlgorithm.Length}-bit salts!");
                }
            }
        }

        [Test]
        [TestCase("one", "two", 8, "SHA512", 1, 512)]
        [TestCase("one", "One", 8, "SHA512", 1, 512)]
        public void DifferentDataShouldProduceDifferentDigest(
            string message1, string message2, int saltLength, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), 1, _saltGenerator);

            var data1 = StringToBytes(message1);
            var data2 = StringToBytes(message2);
            var salt = cryptor.GenerateSalt(saltLength);
            var digest1 = cryptor.DigestBytes(data1, salt, algorithm, iterations, digestLength);
            var digest2 = cryptor.DigestBytes(data2, salt, algorithm, iterations, digestLength);

            Assert.AreNotEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 0, "SHA512", 1, 512)]
        [TestCase("secret", 7, "SHA512", 1, 512)]
        public void InvalidSaltLengthShouldThrow(
            string message, int saltBytes, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations, _saltGenerator);

            var data = StringToBytes(message);
            var salt = cryptor.GenerateSalt(saltBytes);

            Assert.Throws<ArgumentException>(() =>
                cryptor.DigestBytes(data, salt, algorithm, iterations, digestLength));
        }

        [Test]
        [TestCase("secret", 8, 16, "SHA512", 1, 512)]
        public void DifferentSaltLengthsShouldProduceDifferentDigest(
            string message, int salt1Length, int salt2Length, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations, _saltGenerator);

            var data = StringToBytes(message);
            var salt1 = new byte[salt1Length];
            var salt2 = new byte[salt2Length];
            var digest1 = cryptor.DigestBytes(data, salt1, algorithm, iterations, digestLength);
            var digest2 = cryptor.DigestBytes(data, salt2, algorithm, iterations, digestLength);

            Assert.AreNotEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 8, "SHA512", 1, 512)]
        [TestCase("secret", 512, "SHA512", 1, 512)]
        public void DifferentSaltsShouldProduceDifferentDigest(
            string message, int saltBytes, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations, _saltGenerator);

            var data = StringToBytes(message);
            var salt1 = cryptor.GenerateSalt(saltBytes);
            var salt2 = cryptor.GenerateSalt(saltBytes);
            var digest1 = cryptor.DigestBytes(data, salt1, algorithm, iterations, digestLength);
            var digest2 = cryptor.DigestBytes(data, salt2, algorithm, iterations, digestLength);

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
            var cryptor1 = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm1), iterations, _saltGenerator);
            var cryptor2 = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm2), iterations, _saltGenerator);

            var data = StringToBytes(message);
            var salt = cryptor1.GenerateSalt(saltBytes);
            var digest1 = cryptor1.DigestBytes(data, salt, algorithm1, iterations, digestLength);
            var digest2 = cryptor2.DigestBytes(data, salt, algorithm2, iterations, digestLength);

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
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations, _saltGenerator);

            var data = StringToBytes(message);
            var salt = cryptor.GenerateSalt(saltBytes);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                cryptor.DigestBytes(data, salt, algorithm, iterations, digestLength));
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
            var cryptor1 = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations1, _saltGenerator);
            var cryptor2 = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations2, _saltGenerator);

            var data = StringToBytes(message);
            var salt = cryptor1.GenerateSalt(saltBytes);
            var digest1 = cryptor1.DigestBytes(data, salt, algorithm, iterations1, digestLength);
            var digest2 = cryptor2.DigestBytes(data, salt, algorithm, iterations2, digestLength);

            Assert.AreNotEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 8, "SHA512", 1, -1)]
        [TestCase("secret", 8, "SHA512", 1, 0)]
        public void InvalidDigestLengthShouldThrow(
            string message, int saltBytes, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations, _saltGenerator);

            var data = StringToBytes(message);
            var salt = cryptor.GenerateSalt(saltBytes);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                cryptor.DigestBytes(data, salt, algorithm, iterations, digestLength));
        }

        [Test]
        [TestCase("secret", 8, "SHA512", 1, 512, 256)]
        [TestCase("secret", 8, "SHA512", 1, 512, 1024)]
        public void DifferentDigestLengthsShouldProduceDifferentDigest(
            string message, int saltBytes, string algorithm, int iterations, int digestLength1, int digestLength2
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations, _saltGenerator);

            var data = StringToBytes(message);
            var salt = cryptor.GenerateSalt(saltBytes);
            var digest1 = cryptor.DigestBytes(data, salt, algorithm, iterations, digestLength1);
            var digest2 = cryptor.DigestBytes(data, salt, algorithm, iterations, digestLength2);

            Assert.AreNotEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 8, "SHA512", 1, 512)]
        public void SameSettingsShouldProduceSameDigest(
            string message, int saltLength, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations, _saltGenerator);

            var data = StringToBytes(message);
            var salt = cryptor.GenerateSalt(saltLength);
            var digest1 = cryptor.DigestBytes(data, salt, algorithm, iterations, digestLength);
            var digest2 = cryptor.DigestBytes(data, salt, algorithm, iterations, digestLength);

            Assert.AreEqual(digest1, digest2);
        }

        [Test]
        [TestCase("secret", 8, "SHA512", 1, 512)]
        public void ShouldConvertTextFromUnicode(
            string text, int saltBytes, string algorithm, int iterations, int digestLength
        )
        {
            var cryptor = new Pbkdf2SaltedCryptor(HashAlgorithm.Parse(algorithm), iterations, _saltGenerator);

            var salt = cryptor.GenerateSalt(saltBytes);
            var digestText = cryptor.DigestText(text, salt, algorithm, iterations, digestLength);
            var digestBytes = cryptor.DigestBytes(StringToBytes(text), salt, algorithm, iterations, digestLength);

            Assert.AreEqual(digestBytes, digestText);
        }

        private static byte[] StringToBytes(string message)
        {
            return Encoding.Unicode.GetBytes(message);
        }

        private static int BitsToBytes(int keyWidth)
        {
            return keyWidth / 8;
        }
    }
}
