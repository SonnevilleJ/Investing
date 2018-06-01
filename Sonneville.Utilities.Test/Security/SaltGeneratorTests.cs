using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Utilities.Security;

namespace Sonneville.Utilities.Test.Security
{
    [TestFixture]
    public class SaltGeneratorTests
    {
        private ISaltGenerator _saltGenerator;

        [SetUp]
        public void Setup()
        {
            _saltGenerator = new SaltGenerator();
        }

        [Test]
        [TestCase(1000000, 512)]
        public void ShouldGenerateRandomSalt(int iterations, int keyLength)
        {
            var salts = new HashSet<byte[]>();
            for (var i = 0; i < iterations; i++)
            {
                if (!salts.Add(_saltGenerator.GenerateSalt(BitsToBytes(keyLength))))
                {
                    // Due to salt generation being random, duplicates are extremely rare but aren't impossible
                    Assert.Fail($"Duplicate detected after {salts.Count} {keyLength}-bit salts!");
                }
            }
        }

        private static int BitsToBytes(int keyWidth)
        {
            return keyWidth / 8;
        }
    }
}