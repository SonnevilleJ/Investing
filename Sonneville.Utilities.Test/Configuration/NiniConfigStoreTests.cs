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

        [SetUp]
        public void Setup()
        {
            _location = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(NiniConfigStoreTests)}.ini"
            );
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
            var configStore = new NiniConfigStore(_location);
            Assert.False(File.Exists(_location));

            var sampleConfig = configStore.Read<SampleConfig>();

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
            var configStore = new NiniConfigStore(_location);
            var sampleConfig = new SampleConfig
            {
                A = value,
            };

            configStore.Save(sampleConfig);
            var result = configStore.Read<SampleConfig>();

            Assert.AreEqual(sampleConfig.A, result.A);
        }

        [Test]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void ShouldRoundtripInteger(int value)
        {
            var configStore = new NiniConfigStore(_location);
            var sampleConfig = new SampleConfig
            {
                B = value,
            };

            configStore.Save(sampleConfig);
            var result = configStore.Read<SampleConfig>();

            Assert.AreEqual(sampleConfig.B, result.B);
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(0)]
        [TestCase(long.MaxValue)]
        public void ShouldRoundtripLong(long value)
        {
            var configStore = new NiniConfigStore(_location);
            var sampleConfig = new SampleConfig
            {
                C = value,
            };

            configStore.Save(sampleConfig);
            var result = configStore.Read<SampleConfig>();

            Assert.AreEqual(sampleConfig.C, result.C, 0.00001);
        }

        [Test]
        [TestCase(-1234567890)]
        [TestCase(0)]
        [TestCase(1234567890)]
        public void ShouldRoundtripDouble(double value)
        {
            var configStore = new NiniConfigStore(_location);
            var sampleConfig = new SampleConfig
            {
                D = value,
            };

            configStore.Save(sampleConfig);
            var result = configStore.Read<SampleConfig>();

            Assert.AreEqual(sampleConfig.D, result.D);
        }

        [Test]
        public void ShouldReadExistingConfig()
        {
            if (File.Exists(_location)) File.Delete(_location);
            var configStore = new NiniConfigStore(_location);
            var sampleConfig = new SampleConfig
            {
                A = "original",
            };

            configStore.Save(sampleConfig);

            configStore = new NiniConfigStore(_location);
            sampleConfig.A = "changed";

            configStore.Save(sampleConfig);

            var result = configStore.Read<SampleConfig>();
            Assert.AreEqual(sampleConfig.A, result.A);
        }

        [Test]
        public void ShouldDeleteConfigFile()
        {
            var configStore = new NiniConfigStore(_location);
            var sampleConfig = new SampleConfig
            {
                A = "original",
            };
            configStore.Save(sampleConfig);
            Assert.True(File.Exists(_location));

            configStore.DeleteAll();
            
            Assert.False(File.Exists(_location));
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
