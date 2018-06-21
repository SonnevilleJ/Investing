using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Sonneville.Utilities.Security;

namespace Sonneville.Utilities.Test.Security
{
    [TestFixture]
    public class HashAlgorithmTests
    {
        [Test]
        [TestCase("SHA1", 160)]
        [TestCase("SHA256", 256)]
        [TestCase("SHA384", 384)]
        [TestCase("SHA512", 512)]
        public void ShouldParse(string algorithm, int bytes)
        {
            var actual = HashAlgorithm.Parse(algorithm);

            AssertMatches(algorithm, bytes, actual);
        }

        [Test]
        [TestCase("MD5")]
        [TestCase("SHA0")]
        public void ShouldThrowIfUnableToParse(string algorithm)
        {
            Assert.Throws<NotSupportedException>(() => HashAlgorithm.Parse(algorithm));
        }

        [Test]
        [TestCase("SHA1", 160)]
        [TestCase("SHA256", 256)]
        [TestCase("SHA384", 384)]
        [TestCase("SHA512", 512)]
        public void ShouldProvideStaticGenerators(string algorithm, int bytes)
        {
            var actual = FetchHashAlgorithmByStaticField(algorithm);

            AssertMatches(algorithm, bytes, actual);
        }

        private static HashAlgorithm FetchHashAlgorithmByStaticField(string algorithm)
        {
            var propertyInfo = typeof(HashAlgorithm).GetFields(BindingFlags.Static | BindingFlags.Public)
                .Single(property => property.Name == algorithm);
            return propertyInfo.GetValue(null) as HashAlgorithm;
        }

        private static void AssertMatches(string expectedName, int expectedLength, HashAlgorithm actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedName, actual.HashAlgorithmName.Name);
            Assert.AreEqual(expectedName, actual.Name);
            Assert.AreEqual(expectedLength, actual.Length);
        }
    }
}