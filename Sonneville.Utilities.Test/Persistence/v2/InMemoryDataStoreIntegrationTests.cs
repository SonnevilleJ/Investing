using System;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public class InMemoryDataStoreIntegrationTests
    {
        private InMemoryDataStore _store;

        [SetUp]
        public void Setup()
        {
            _store = new InMemoryDataStore();
            _store.DeleteAll();
        }

        [Test]
        public void GetShouldLoadIfNotCached()
        {
            _store.Save(new SampleData
            {
                A = "original",
            });

            var configStore = new InMemoryDataStore();
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

            var configStore = new InMemoryDataStore();
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

            var configStore = new InMemoryDataStore();
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
            var data = _store.Load<SampleData>();

            AssertDefaultConfig(data);
        }

        [Test]
        public void DeleteAllShouldEmptyCache()
        {
            var data = new SampleData
            {
                A = "original",
            };
            _store.Save(data);

            _store.DeleteAll();

            AssertDefaultConfig(_store.Get<SampleData>());
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