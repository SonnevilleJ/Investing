using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Utilities.Test.Configuration
{
    [TestFixture]
    public class JsonConfigStoreTests
    {
        private string _path;
        private JsonConfigStore _configStore;

        [SetUp]
        public void Setup()
        {
            _path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(JsonConfigStoreTests)}.ini"
            );
            _configStore = new JsonConfigStore(_path);
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_path)) File.Delete(_path);
            Assert.False(File.Exists(_path));
        }

        [Test]
        public void GetShouldLoadIfNotCached()
        {
            _configStore.Save(new SampleConfig
            {
                A = "original",
            });

            var configStore = new JsonConfigStore(_path);
            var one = configStore.Get<SampleConfig>();
            var two = configStore.Get<SampleConfig>();

            Assert.AreEqual("original", one.A);
            Assert.AreEqual("original", two.A);
            Assert.AreSame(one, two);
        }

        [Test]
        public void GetShouldNotLoadAndReturnCachedIfCached()
        {
            var sampleConfig = new SampleConfig
            {
                A = "original",
            };
            _configStore.Save(sampleConfig);
            sampleConfig.A = "changed";

            var result = _configStore.Get<SampleConfig>();
            Assert.AreEqual("changed", result.A);
            Assert.AreSame(sampleConfig, result);
        }

        [Test]
        public void LoadShouldUpdateAndReturnPersistedConfig()
        {
            var sampleConfig = new SampleConfig
            {
                A = "original",
            };
            _configStore.Save(sampleConfig);
            sampleConfig.A = "changed";

            var result = _configStore.Load<SampleConfig>();
            Assert.AreEqual("original", result.A);
            Assert.AreSame(sampleConfig, result);
        }

        [Test]
        public void LoadShouldReturnDefaultConfigdWhenNoFilePresent()
        {
            Assert.False(File.Exists(_path));

            var sampleConfig = _configStore.Load<SampleConfig>();

            Assert.NotNull(sampleConfig);
            foreach (var propertyInfo in typeof(SampleConfig).GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                var defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
                var actualValue = propertyInfo.GetValue(sampleConfig);
                Assert.AreEqual(defaultValue, actualValue);
            }
        }

        [Test]
        public void DeleteAllShouldDeleteFile()
        {
            var sampleConfig = new SampleConfig
            {
                A = "original",
            };
            _configStore.Save(sampleConfig);
            Assert.True(File.Exists(_path));

            _configStore.DeleteAll();

            Assert.False(File.Exists(_path));
        }

        [Test]
        public void SaveShouldThrowIfSavingDifferentConfigOfSameType()
        {
            _configStore.Get<SampleConfig>();

            var imposter = new SampleConfig();
            Assert.Throws<ArgumentOutOfRangeException>(() => _configStore.Save(imposter));
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
            var result = _configStore.Get<SampleConfig>();

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
            var result = _configStore.Get<SampleConfig>();

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
            var result = _configStore.Get<SampleConfig>();

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
            var result = _configStore.Get<SampleConfig>();

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
            var result = _configStore.Get<SampleConfig>();

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
            var result = _configStore.Get<SampleConfig>();

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
            var result = _configStore.Get<SampleConfig>();

            Assert.AreEqual(sampleConfig.G, result.G);
        }

        private class SampleConfig
        {
            public string A { get; set; }

            public int B { get; set; }

            public long C { get; set; }

            public double D { get; set; }

            public decimal E { get; set; }

            public TimeSpan F { get; set; }

            public HashSet<string> G { get; set; }
        }
    }
}