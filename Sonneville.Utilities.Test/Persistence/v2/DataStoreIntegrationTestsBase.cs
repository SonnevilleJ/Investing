using System;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public abstract class DataStoreIntegrationTestsBase
    {
        private IDataStore _store;

        protected abstract DataStore InstantiateDataStore();

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
        public void GetShouldLoadIfNotCached()
        {
            SetupPersistedData();

            var configStore = InstantiateDataStore();
            var one = configStore.Get<SampleData>();
            var two = configStore.Get<SampleData>();

            Assert.AreEqual("original", one.A);
            Assert.AreEqual("original", two.A);
            Assert.AreSame(one, two);
        }

        [Test]
        public void GetShouldNotLoadAndReturnCachedIfCached()
        {
            var data = SetupPersistedData();
            data.A = "changed";

            var result = _store.Get<SampleData>();

            Assert.AreEqual("changed", result.A);
            Assert.AreSame(data, result);
        }

        [Test]
        public void GetShouldReturnDefaultIfNotPersisted()
        {
            SetupPersistedData();

            var configStore = InstantiateDataStore();
            var data = configStore.Get<OtherData>();

            AssertDefaultConfig(data);
        }

        [Test]
        public void LoadShouldReturnDefaultIfNotPersisted()
        {
            SetupPersistedData();

            var configStore = InstantiateDataStore();
            var data = configStore.Load<OtherData>();

            AssertDefaultConfig(data);
        }

        [Test]
        public void LoadShouldUpdateAndReturnPersistedConfig()
        {
            var data = SetupPersistedData();
            data.A = "changed";

            var result = _store.Load<SampleData>();

            Assert.AreEqual("original", result.A);
            Assert.AreSame(data, result);
        }

        [Test]
        public void LoadShouldReturnDefaultConfigWhenNothingIsPersisted()
        {
            var data = _store.Load<SampleData>();

            AssertDefaultConfig(data);
        }

        [Test]
        public void DeleteAllShouldEmptyCache()
        {
            SetupPersistedData();

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

        [Test]
        public void CachedCountShouldCountCachedItems()
        {
            Assert.AreEqual(0, _store.CachedCount);
            
            SetupPersistedData();

            Assert.AreEqual(1, _store.CachedCount);
        }

        private SampleData SetupPersistedData()
        {
            var data = new SampleData
            {
                A = "original",
            };
            _store.Save(data);
            return data;
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