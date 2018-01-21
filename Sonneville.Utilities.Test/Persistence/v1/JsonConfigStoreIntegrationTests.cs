using System;
using System.IO;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v1;

namespace Sonneville.Utilities.Test.Persistence.v1
{
    [TestFixture]
    public class JsonConfigStoreIntegrationTests
    {
        private string _path;
        private JsonConfigStore<SampleConfig> _configStore;

        [SetUp]
        public void Setup()
        {
            _path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(JsonConfigStoreIntegrationTests)}.json"
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
        public void GetShouldLoadIfNotCached()
        {
            _configStore.Save(new SampleConfig
            {
                A = "original",
            });

            var configStore = new JsonConfigStore<SampleConfig>(_path);
            var one = configStore.Get();
            var two = configStore.Get();

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

            var result = _configStore.Get();
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

            var result = _configStore.Load();
            Assert.AreEqual("original", result.A);
            Assert.AreSame(sampleConfig, result);
        }

        [Test]
        public void LoadShouldReturnDefaultConfigdWhenNoFilePresent()
        {
            Assert.False(File.Exists(_path));

            var sampleConfig = _configStore.Load();

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
            _configStore.Get();

            var imposter = new SampleConfig();
            Assert.Throws<ArgumentOutOfRangeException>(() => _configStore.Save(imposter));
        }
    }
}