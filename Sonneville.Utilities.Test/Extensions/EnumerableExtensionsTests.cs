using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sonneville.Utilities.Extensions;

namespace Sonneville.Utilities.Test.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [TestFixture]
        public class ZipAllExtensionTests
        {
            [Test]
            public void ZipAllShouldZipEqualSizedEnumerables()
            {
                IEnumerable<string> enumerable1 = new List<string> {"a", "b", "c"};
                IEnumerable<string> enumerable2 = new List<string> {"x", "y", "z"};
                var list1 = enumerable1.ToList();
                var list2 = enumerable2.ToList();
                var iteration = 0;

                Func<string, string, string> resultSelector =
                    (one, two) => VerifyArguments(list1, one, list2, two, ref iteration);
                var result = enumerable1.ZipAll(enumerable2, resultSelector);

                Assert.AreEqual(Math.Max(list1.Count, list2.Count), result.Count());
            }

            [Test]
            public void ZipAllShouldZipInequalSizedEnumerablesOne()
            {
                var list1 = new List<string> {"a", "b", "c", "d"};
                var list2 = new List<string> {"x", "y", "z"};
                var iteration = 0;

                Func<string, string, string> resultSelector =
                    (one, two) => VerifyArguments(list1, one, list2, two, ref iteration);
                var result = list1.ZipAll(list2, resultSelector);

                Assert.AreEqual(Math.Max(list1.Count, list2.Count), result.Count());
            }

            [Test]
            public void ZipAllShouldZipInequalSizedEnumerablesTwo()
            {
                var list1 = new List<string> {"a", "b", "c"};
                var list2 = new List<string> {"x", "y", "z", "d"};
                var iteration = 0;

                Func<string, string, string> resultSelector =
                    (one, two) => VerifyArguments(list1, one, list2, two, ref iteration);
                var result = list1.ZipAll(list2, resultSelector);

                Assert.AreEqual(Math.Max(list1.Count, list2.Count), result.Count());
            }

            [Test]
            public void ZipAllShouldDisposeEachEnumerable()
            {
                var e1EnumeratorMock = new Mock<IEnumerator<string>>();
                e1EnumeratorMock.Setup(enumerator => enumerator.Dispose()).Verifiable();
                var e1Mock = new Mock<IEnumerable<string>>(MockBehavior.Strict);
                e1Mock.Setup(enumerable => enumerable.GetEnumerator()).Returns(e1EnumeratorMock.Object);
                var e2EnumeratorMock = new Mock<IEnumerator<string>>();
                e2EnumeratorMock.Setup(enumerator => enumerator.Dispose()).Verifiable();
                e2EnumeratorMock.Setup(enumerator => enumerator.Current).Throws<Exception>();
                var e2Mock = new Mock<IEnumerable<string>>(MockBehavior.Strict);
                e2Mock.Setup(enumerable => enumerable.GetEnumerator()).Returns(e2EnumeratorMock.Object);

                var results = e1Mock.Object.ZipAll(e2Mock.Object, (one, two) => string.Empty);

                Assert.AreEqual(0, results.Count());
                e1EnumeratorMock.Verify();
                e2EnumeratorMock.Verify();
            }

            [Test]
            public void ZipAllShouldSafelyDisposeFirstEnumerable()
            {
                var e1EnumeratorMock = new Mock<IEnumerator<string>>();
                e1EnumeratorMock.Setup(enumerator => enumerator.MoveNext()).Returns(true);
                e1EnumeratorMock.Setup(enumerator => enumerator.Dispose()).Verifiable();
                var e1Mock = new Mock<IEnumerable<string>>(MockBehavior.Strict);
                e1Mock.Setup(enumerable => enumerable.GetEnumerator()).Returns(e1EnumeratorMock.Object);
                var e2Mock = new Mock<IEnumerable<string>>(MockBehavior.Strict);
                e2Mock.Setup(enumerable => enumerable.GetEnumerator()).Returns((IEnumerator<string>) null);

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Assert.Throws<NullReferenceException>(
                    () => e1Mock.Object.ZipAll(e2Mock.Object, (one, two) => string.Empty).ToList());

                e1EnumeratorMock.Verify();
            }

            [Test]
            public void ZipAllShouldSafelyDisposeSecondEnumerable()
            {
                var e1Mock = new Mock<IEnumerable<string>>(MockBehavior.Strict);
                e1Mock.Setup(enumerable => enumerable.GetEnumerator()).Returns((IEnumerator<string>) null);
                var e2EnumeratorMock = new Mock<IEnumerator<string>>();
                e2EnumeratorMock.Setup(enumerator => enumerator.Dispose()).Verifiable();
                var e2Mock = new Mock<IEnumerable<string>>(MockBehavior.Strict);
                e2Mock.Setup(enumerable => enumerable.GetEnumerator()).Returns(e2EnumeratorMock.Object);

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Assert.Throws<NullReferenceException>(
                    () => e1Mock.Object.ZipAll(e2Mock.Object, (one, two) => string.Empty).ToList());

                e2EnumeratorMock.Verify();
            }

            private static string VerifyArguments(IReadOnlyList<string> list1, string one, IReadOnlyList<string> list2,
                string two, ref int iteration)
            {
                if (list1.Count > iteration) Assert.AreEqual(list1[iteration], one);
                if (list2.Count > iteration) Assert.AreEqual(list2[iteration], two);
                iteration++;
                return one + two;
            }
        }
    }
}