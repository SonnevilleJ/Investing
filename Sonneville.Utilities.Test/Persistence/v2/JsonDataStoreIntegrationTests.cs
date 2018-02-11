using System;
using System.IO;
using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public class JsonDataStoreIntegrationTests
    {
        private Mock<ILog> _logMock;
        private string _path;
        private JsonDataStore _store;

        [SetUp]
        public void Setup()
        {
            _path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(JsonDataStoreIntegrationTests)}.json"
            );
            Console.WriteLine($"Path used for tests: {_path}");

            _logMock = new Mock<ILog>();
            
            _store = new JsonDataStore(_logMock.Object, _path);
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

            var configStore = new JsonDataStore(_logMock.Object, _path);
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
        public void GetShouldReturnDefaultIfNotPersisted()
        {
            _store.Save(new SampleData
            {
                A = "original",
            });

            var configStore = new JsonDataStore(_logMock.Object, _path);
            var data = configStore.Get<OtherData>();

            AssertDefaultConfig(data);
        }

        [Test]
        public void LoadShouldReturnDefaultIfNotPersisted()
        {
            _store.Save(new SampleData
            {
                A = "original",
            });

            var configStore = new JsonDataStore(_logMock.Object, _path);
            var data = configStore.Load<OtherData>();

            AssertDefaultConfig(data);
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

            AssertDefaultConfig(data);
        }

        [Test]
        public void LoadShouldFailIfNewerVersionDetected()
        {
            _store.Save(new SampleData());

            var fileContents = File.ReadAllText(_path);
            File.WriteAllText(_path, fileContents.Replace(JsonDataStore.DataStoreVersion, "asdf"));

            Assert.Throws<NotSupportedException>(() => _store.Load<SampleData>());
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

        private static void AssertDefaultConfig<T>(T data)
        {
            Assert.NotNull(data);
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                var defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
                var actualValue = propertyInfo.GetValue(data);
                Assert.AreEqual(defaultValue, actualValue);
            }
        }
    }
}