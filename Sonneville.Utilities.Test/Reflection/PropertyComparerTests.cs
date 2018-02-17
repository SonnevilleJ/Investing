using NUnit.Framework;
using Sonneville.Utilities.Reflection;

namespace Sonneville.Utilities.Test.Reflection
{
    [TestFixture]
    public class PropertyComparerTests
    {
        private PropertyComparer _propertyComparer;

        [SetUp]
        public void Setup()
        {
            _propertyComparer = new PropertyComparer();
        }

        [Test]
        public void LeftNullReturnsNotEqual()
        {
            var compare = _propertyComparer.Compare(null, 1);

            Assert.AreNotEqual(0, compare);
        }

        [Test]
        public void RightNullReturnsNotEqual()
        {
            var compare = _propertyComparer.Compare(1, null);

            Assert.AreNotEqual(0, compare);
        }

        [Test]
        public void DifferentTypesReturnsNotEqual()
        {
            var compare = _propertyComparer.Compare(1, 1L);

            Assert.AreNotEqual(0, compare);
        }

        [Test]
        public void SameTypeSameValueReturnsEqualForValueType()
        {
            var compare = _propertyComparer.Compare(1, 1);

            Assert.AreEqual(0, compare);
        }

        [Test]
        public void SameTypeDifferentValueReturnsNotEqualForValueTypes()
        {
            var compare = _propertyComparer.Compare(1, 2);

            Assert.AreNotEqual(0, compare);
        }

        [Test]
        public void SameTypeSameValueReturnsEqualForReferenceType()
        {
            var compare = _propertyComparer.Compare("1", "1");

            Assert.AreEqual(0, compare);
        }

        [Test]
        public void SameTypeDifferentValueReturnsNotEqualForReferenceType()
        {
            var compare = _propertyComparer.Compare("1", "2");

            Assert.AreEqual(0, compare);
        }
    }
}
