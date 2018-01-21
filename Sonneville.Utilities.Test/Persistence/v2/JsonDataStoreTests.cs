using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v1;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public class JsonDataStoreTests
    {
        private string _path;
        private JsonDataStore _store;

        [SetUp]
        public void Setup()
        {
            _path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(JsonDataStoreTests)}.json"
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
        public void GetShouldLoadIfNotCached()
        {
            _store.Save(new SampleData
            {
                A = "original",
            });

            var configStore = new JsonDataStore(_path);
            var one = configStore.Get<SampleData>();
            var two = configStore.Get<SampleData>();

            Assert.AreEqual("original", one.A);
            Assert.AreEqual("original", two.A);
            Assert.AreSame(one, two);
        }

        [Test]
        public void GetShouldNotLoadAndReturnCachedIfCached()
        {
            var data = new SampleData
            {
                A = "original",
            };
            _store.Save(data);
            data.A = "changed";

            var result = _store.Get<SampleData>();
            Assert.AreEqual("changed", result.A);
            Assert.AreSame(data, result);
        }

        [Test]
        public void LoadShouldUpdateAndReturnPersistedConfig()
        {
            var data = new SampleData
            {
                A = "original",
            };
            _store.Save(data);
            data.A = "changed";

            var result = _store.Load<SampleData>();
            Assert.AreEqual("original", result.A);
            Assert.AreSame(data, result);
        }

        [Test]
        public void LoadShouldReturnDefaultConfigdWhenNoFilePresent()
        {
            Assert.False(File.Exists(_path));

            var data = _store.Load<SampleData>();

            Assert.NotNull(data);
            foreach (var propertyInfo in typeof(SampleData).GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                var defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
                var actualValue = propertyInfo.GetValue(data);
                Assert.AreEqual(defaultValue, actualValue);
            }
        }

        [Test]
        public void DeleteAllShouldDeleteFile()
        {
            var data = new SampleData
            {
                A = "original",
            };
            _store.Save(data);
            Assert.True(File.Exists(_path));

            _store.DeleteAll();

            Assert.False(File.Exists(_path));
        }

        [Test]
        public void SaveShouldThrowIfSavingDifferentConfigOfSameType()
        {
            _store.Get<SampleData>();

            var imposter = new SampleData();
            Assert.Throws<ArgumentOutOfRangeException>(() => _store.Save(imposter));
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

        [Test]
        public void LoadShouldUpgradeOldPersistedData()
        {
            var sampleData = CreateLegacySampleData();

            var legacyConfigStore = new JsonConfigStore(_path);
            legacyConfigStore.Save(sampleData);
            Assert.AreEqual(sampleData.A, legacyConfigStore.Get<SampleData>().A);
            
            var retrievedSampleData = _store.Get<SampleData>();
            Assert.AreEqual(sampleData.A, retrievedSampleData.A);
            Assert.AreEqual(sampleData.B, retrievedSampleData.B);
            Assert.AreEqual(sampleData.C, retrievedSampleData.C);
            Assert.AreEqual(sampleData.D, retrievedSampleData.D);
            Assert.AreEqual(sampleData.E, retrievedSampleData.E);
            Assert.AreEqual(sampleData.F, retrievedSampleData.F);
            Assert.AreEqual(sampleData.G, retrievedSampleData.G);
            Assert.AreEqual(sampleData.H, retrievedSampleData.H);
            Assert.AreEqual(sampleData.I, retrievedSampleData.I);
        }

        [Test]
        public void LoadShouldThrowIfNewerVersionDetected()
        {
            var jObject = JObject.FromObject(new {Version = "v6"});
            File.WriteAllText(_path, jObject.ToString());

            Assert.Throws<NotSupportedException>(() => _store.Load<SampleData>());
        }

        private static SampleData CreateLegacySampleData()
        {
            return new SampleData
            {
                A = "sample",
                B = 5,
                C = 500,
                D = 1234.56,
                E = 123.4567m,
                F = new TimeSpan(5, 1, 2, 3),
                G = new HashSet<string>(new[] {"backward compatibility FTW"}),
                H = new Dictionary<Type, object> {{typeof(string), "hi!"}},
                I = new List<string> {"one", "two", "three"},
            };
        }

        private class SampleData
        {
            public string A { get; set; }

            public int B { get; set; }

            public long C { get; set; }

            public double D { get; set; }

            public decimal E { get; set; }

            public TimeSpan F { get; set; }

            public HashSet<string> G { get; set; }

            public Dictionary<Type, object> H { get; set; }

            public List<string> I { get; set; }
        }

        private class OtherData
        {
            public string A { get; set; }
        }
    }
}