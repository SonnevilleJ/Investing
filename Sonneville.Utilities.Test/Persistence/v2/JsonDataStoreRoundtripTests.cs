using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    public class JsonDataStoreRoundtripTests
    {
        private string _path;
        private JsonDataStore _store;

        [SetUp]
        public void Setup()
        {
            _path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(JsonDataStoreRoundtripTests)}.json"
            );
            Console.WriteLine($"Path used for tests: {_path}");
            _store = new JsonDataStore(_path);
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_path))
            {
                Console.WriteLine(
                    $"Clearing persisted data... Contents follow: {Environment.NewLine}{File.ReadAllText(_path)}");
                File.Delete(_path);
            }

            Assert.False(File.Exists(_path));
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
            var result = new JsonDataStore(_path).Load<SampleData>();

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
            var result = new JsonDataStore(_path).Load<SampleData>();

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
            var result = new JsonDataStore(_path).Load<SampleData>();

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
            var result = new JsonDataStore(_path).Load<SampleData>();

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
            var result = new JsonDataStore(_path).Load<SampleData>();

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
            var result = new JsonDataStore(_path).Load<SampleData>();

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
            var result = new JsonDataStore(_path).Load<SampleData>();

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
            var result = new JsonDataStore(_path).Load<SampleData>();

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
            var result = new JsonDataStore(_path).Load<SampleData>();

            Assert.AreEqual(data.I, result.I);
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
            var retrievedSampleData = new JsonDataStore(_path).Get<SampleData>();
            var retrievedOtherData = new JsonDataStore(_path).Get<OtherData>();

            Assert.AreEqual(sampleData.A, retrievedSampleData.A);
            Assert.AreEqual(otherData.A, retrievedOtherData.A);
        }
    }
}