using System;
using System.IO;
using NUnit.Framework;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Utilities.Test.Configuration
{
    [TestFixture]
    public class NiniConfigStoreTests
    {
        private string _location;
        private NiniConfigStore _configStore;

        [SetUp]
        public void Setup()
        {
            _location = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(NiniConfigStoreTests)}.ini"
            );
            _configStore = new NiniConfigStore(_location);
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_location)) File.Delete(_location);
            Assert.False(File.Exists(_location));
        }

        [Test]
        public void ShouldReadWhenNoFilePresent()
        {
            Assert.False(File.Exists(_location));

            var sampleConfig = _configStore.Read<SampleConfig>();

            Assert.NotNull(sampleConfig);
            Assert.AreEqual(default(string), sampleConfig.A);
            Assert.AreEqual(default(int), sampleConfig.B);
            Assert.AreEqual(default(long), sampleConfig.C);
            Assert.AreEqual(default(double), sampleConfig.D);
        }

        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void ShouldRoundtripString(string value)
        {
            var sampleConfig = new SampleConfig
            {
                A = value,
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Read<SampleConfig>();

            Assert.AreEqual(sampleConfig.A, result.A);
        }

        [Test]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void ShouldRoundtripInteger(int value)
        {
            var sampleConfig = new SampleConfig
            {
                B = value,
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Read<SampleConfig>();

            Assert.AreEqual(sampleConfig.B, result.B);
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(0)]
        [TestCase(long.MaxValue)]
        public void ShouldRoundtripLong(long value)
        {
            var sampleConfig = new SampleConfig
            {
                C = value,
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Read<SampleConfig>();

            Assert.AreEqual(sampleConfig.C, result.C, 0.00001);
        }

        [Test]
        [TestCase(-1234567890)]
        [TestCase(0)]
        [TestCase(1234567890)]
        public void ShouldRoundtripDouble(double value)
        {
            var sampleConfig = new SampleConfig
            {
                D = value,
            };

            _configStore.Save(sampleConfig);
            var result = _configStore.Read<SampleConfig>();

            Assert.AreEqual(sampleConfig.D, result.D);
        }

        [Test]
        public void ShouldReadExistingConfig()
        {
            var sampleConfig = new SampleConfig
            {
                A = "original",
            };

            _configStore.Save(sampleConfig);

            _configStore = new NiniConfigStore(_location);
            sampleConfig.A = "changed";

            _configStore.Save(sampleConfig);

            var result = _configStore.Read<SampleConfig>();
            Assert.AreEqual(sampleConfig.A, result.A);
        }

        [Test]
        public void ShouldDeleteConfigFile()
        {
            var sampleConfig = new SampleConfig
            {
                A = "original",
            };
            _configStore.Save(sampleConfig);
            Assert.True(File.Exists(_location));

            _configStore.DeleteAll();
            
            Assert.False(File.Exists(_location));
        }

        [Test]
        public void ShouldCacheConfig()
        {
            Assert.False(File.Exists(_location));

            Assert.AreSame(_configStore.Read<SampleConfig>(), _configStore.Read<SampleConfig>());
        }

        [Test]
        public void ShouldThrowIfSavingDifferentConfigOfSameType()
        {
            _configStore.Read<SampleConfig>();

            var imposter = new SampleConfig();
            Assert.Throws<ArgumentOutOfRangeException>(() => _configStore.Save(imposter));
        }

        private class SampleConfig
        {
            public string A { get; set; }

            public int B { get; set; }

            public long C { get; set; }

            public double D { get; set; }
        }
    }
}
