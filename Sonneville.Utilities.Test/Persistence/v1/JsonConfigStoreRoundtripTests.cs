using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v1;

namespace Sonneville.Utilities.Test.Persistence.v1
{
    [TestFixture]
    public class JsonConfigStoreRoundtripTests
    {
        private string _path;
        private JsonConfigStore<SampleConfig> _configStore;

        [SetUp]
        public void Setup()
        {
            _path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(JsonConfigStoreRoundtripTests)}.json"
            );
            _configStore = new JsonConfigStore<SampleConfig>(_path);
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_path))
            {
                Console.WriteLine($"Clearing persisted data... Contents follow: {Environment.NewLine}{File.ReadAllText(_path)}");
                File.Delete(_path);
            }            Assert.False(File.Exists(_path));
        }
        
        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldRoundtripStringType(string value)
        {
            var sampleConfig = new SampleConfig
            {
                A = value,
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Get();

            Assert.AreEqual(sampleConfig.A, result.A);
        }

        [Test]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void ShouldRoundtripIntegerType(int value)
        {
            var sampleConfig = new SampleConfig
            {
                B = value,
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Get();

            Assert.AreEqual(sampleConfig.B, result.B);
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(0)]
        [TestCase(long.MaxValue)]
        public void ShouldRoundtripLongType(long value)
        {
            var sampleConfig = new SampleConfig
            {
                C = value,
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Get();

            Assert.AreEqual(sampleConfig.C, result.C, 0.00001);
        }

        [Test]
        [TestCase(-1234567890)]
        [TestCase(0)]
        [TestCase(1234567890)]
        public void ShouldRoundtripDoubleType(double value)
        {
            var sampleConfig = new SampleConfig
            {
                D = value,
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Get();

            Assert.AreEqual(sampleConfig.D, result.D);
        }

        [Test]
        [TestCase(-1234567890)]
        [TestCase(0)]
        [TestCase(1234567890)]
        public void ShouldRoundtripDecimalType(decimal value)
        {
            var sampleConfig = new SampleConfig
            {
                E = value,
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Get();

            Assert.AreEqual(sampleConfig.E, result.E);
        }

        [Test]
        [TestCase(-123)]
        [TestCase(0)]
        [TestCase(123)]
        public void ShouldRoundtripTimespanType(int seconds)
        {
            var sampleConfig = new SampleConfig
            {
                F = TimeSpan.FromSeconds(seconds),
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Get();

            Assert.AreEqual(sampleConfig.F, result.F);
        }

        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldRoundtripHashSetType(string value)
        {
            var sampleConfig = new SampleConfig
            {
                G = new HashSet<string>(new[] {value}),
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Get();

            Assert.AreEqual(sampleConfig.G, result.G);
        }

        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldRoundtripDictionaryType(string value)
        {
            var sampleConfig = new SampleConfig
            {
                H = new Dictionary<Type, object>(),
            };
            sampleConfig.H.Add(typeof(string), value);

            _configStore.Save(sampleConfig);
            var result = _configStore.Get();

            Assert.AreEqual(sampleConfig.H, result.H);
        }

        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldRoundtripListType(string value)
        {
            var sampleConfig = new SampleConfig
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

            _configStore.Save(sampleConfig);
            var result = new JsonConfigStore<SampleConfig>(_path).Load();

            Assert.AreEqual(sampleConfig.I, result.I);
        }
    }
}