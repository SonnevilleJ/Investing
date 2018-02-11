using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public abstract class DataStoreRoundtripTestsBase
    {
        private IDataStore _store;

        protected abstract IDataStore InstantiateDataStore();

        [SetUp]
        public virtual void Setup()
        {
            _store = InstantiateDataStore();
        }

        [TearDown]
        public virtual void Teardown()
        {
            _store.DeleteAll();
        }
        
        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldRoundtripStringType(string value)
        {
            var data = new SampleData
            {
                A = value,
            };

            _store.Save(data);
            var result = InstantiateDataStore().Load<SampleData>();

            Assert.AreEqual(data.A, result.A);
        }

        [Test]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void ShouldRoundtripIntegerType(int value)
        {
            var data = new SampleData
            {
                B = value,
            };

            _store.Save(data);
            var result = InstantiateDataStore().Load<SampleData>();

            Assert.AreEqual(data.B, result.B);
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(0)]
        [TestCase(long.MaxValue)]
        public void ShouldRoundtripLongType(long value)
        {
            var data = new SampleData
            {
                C = value,
            };

            _store.Save(data);
            var result = InstantiateDataStore().Load<SampleData>();

            Assert.AreEqual(data.C, result.C, 0.00001);
        }

        [Test]
        [TestCase(-1234567890)]
        [TestCase(0)]
        [TestCase(1234567890)]
        public void ShouldRoundtripDoubleType(double value)
        {
            var data = new SampleData
            {
                D = value,
            };

            _store.Save(data);
            var result = InstantiateDataStore().Load<SampleData>();

            Assert.AreEqual(data.D, result.D);
        }

        [Test]
        [TestCase(-1234567890)]
        [TestCase(0)]
        [TestCase(1234567890)]
        public void ShouldRoundtripDecimalType(decimal value)
        {
            var data = new SampleData
            {
                E = value,
            };

            _store.Save(data);
            var result = InstantiateDataStore().Load<SampleData>();

            Assert.AreEqual(data.E, result.E);
        }

        [Test]
        [TestCase(-123)]
        [TestCase(0)]
        [TestCase(123)]
        public void ShouldRoundtripTimespanType(int seconds)
        {
            var data = new SampleData
            {
                F = TimeSpan.FromSeconds(seconds),
            };

            _store.Save(data);
            var result = InstantiateDataStore().Load<SampleData>();

            Assert.AreEqual(data.F, result.F);
        }

        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldRoundtripHashSetType(string value)
        {
            var data = new SampleData
            {
                G = new HashSet<string>(new[] {value}),
            };

            _store.Save(data);
            var result = InstantiateDataStore().Load<SampleData>();

            Assert.AreEqual(data.G, result.G);
        }

        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldRoundtripDictionaryType(string value)
        {
            var data = new SampleData
            {
                H = new Dictionary<Type, object>(),
            };
            data.H.Add(typeof(string), value);

            _store.Save(data);
            var result = InstantiateDataStore().Load<SampleData>();

            Assert.AreEqual(data.H, result.H);
        }

        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldRoundtripListType(string value)
        {
            var data = new SampleData
            {
                I = new List<string>
                {
                    value,
                    value,
                    value,
                    value,
                    value,
                },
            };

            _store.Save(data);
            var result = InstantiateDataStore().Load<SampleData>();

            CollectionAssert.AreEquivalent(data.I, result.I);
        }

        [Test]
        public void ShouldRoundtripMultipleTypes()
        {
            var sampleData = new SampleData
            {
                A = "sample",
            };
            var otherData = new OtherData
            {
                A = "other",
            };

            _store.Save(sampleData);
            _store.Save(otherData);
            var retrievedSampleData = InstantiateDataStore().Get<SampleData>();
            var retrievedOtherData = InstantiateDataStore().Get<OtherData>();

            Assert.AreEqual(sampleData.A, retrievedSampleData.A);
            Assert.AreEqual(otherData.A, retrievedOtherData.A);
        }
    }
}