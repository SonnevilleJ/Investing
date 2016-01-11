using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Sonneville.Investing.Persistence.Test
{
    [TestFixture]
    public class AllocationRepositoryTests
    {
        private Dictionary<string, Dictionary<string, decimal>> _allocation;
        private AllocationRepository _repository;

        [SetUp]
        public void Setup()
        {
            _allocation = new Dictionary<string, Dictionary<string, decimal>>
            {
                {
                    "account 1",
                    new Dictionary<string, decimal>
                    {
                        {
                            "ticker 1",
                            .5m
                        },
                        {
                            "ticker 2",
                            .5m
                        },
                    }
                },
                {
                    "account 2",
                    new Dictionary<string, decimal>
                    {
                        {
                            "ticker 3",
                            .4m
                        },
                        {
                            "ticker 4",
                            .4m
                        },
                    }
                },
            };

            _repository = new AllocationRepository();

            ClearJsonFiles();
        }

        [TearDown]
        public void Teardown()
        {
            ClearJsonFiles();
        }

        [Test]
        public void ShouldReturnPersistedAllocations()
        {
            _repository.Save("username", _allocation);

            Assert.IsTrue(_repository.Exists("username"));
            var allocation = _repository.Get("username");
            CollectionAssert.AreEquivalent(_allocation, allocation);
        }

        [Test]
        public void ShouldSeparateUserAllocations()
        {
            _repository.Save("user1", _allocation);
            _repository.Save("user2", new Dictionary<string, Dictionary<string, decimal>>());

            var allocation = _repository.Get("user2");

            CollectionAssert.AreNotEquivalent(_allocation, allocation);
        }

        [Test]
        public void ShouldDeletePersistedValue()
        {
            _repository.Save("user1", _allocation);

            _repository.Delete("user1");

            Assert.IsFalse(_repository.Exists("user1"));
        }

        [Test]
        public void ShouldThrowIfNoPersistedAllocationFound()
        {
            Assert.IsFalse(_repository.Exists("asdf"));
            Assert.Throws<KeyNotFoundException>(() => _repository.Get("asdf"));
        }

        private void ClearJsonFiles()
        {
            var root = GetPersistenceStoreRoot();
            var jsonFiles = Directory.GetFiles(root, "*.json");
            foreach (var file in jsonFiles)
            {
                File.Delete(file);
            }
        }

        private string GetPersistenceStoreRoot()
            => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}