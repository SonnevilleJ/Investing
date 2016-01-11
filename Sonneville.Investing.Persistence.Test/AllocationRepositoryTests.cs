using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.Persistence.Test
{
    [TestFixture]
    public class AllocationRepositoryTests
    {
        private Dictionary<string, Allocation> _accountAllocations;
        private IAllocationRepository _repository;

        [SetUp]
        public void Setup()
        {
            _accountAllocations = new Dictionary<string, Allocation>
            {
                {
                    "account 1",
                    Allocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {
                            "ticker 1",
                            .8m
                        },
                        {
                            "ticker 2",
                            .2m
                        },
                    })
                },
                {
                    "account 2",
                    Allocation.FromDictionary(new Dictionary<string, decimal>
                    {
                        {
                            "ticker 3",
                            .6m
                        },
                        {
                            "ticker 4",
                            .4m
                        },
                    })
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
            _repository.Save("username", _accountAllocations);

            Assert.IsTrue(_repository.Exists("username"));
            var allocation = _repository.Get("username");
            foreach (var setupAccountAllocation in _accountAllocations)
            {
                CollectionAssert.AreEquivalent(
                    setupAccountAllocation.Value.ToDictionary(),
                    allocation[setupAccountAllocation.Key].ToDictionary());
            }
        }

        [Test]
        public void ShouldSeparateUserAllocations()
        {
            _repository.Save("user1", _accountAllocations);
            _repository.Save("user2", new Dictionary<string, Allocation>());

            var allocation = _repository.Get("user2");

            CollectionAssert.AreNotEquivalent(_accountAllocations, allocation);
        }

        [Test]
        public void ShouldDeletePersistedValue()
        {
            _repository.Save("user1", _accountAllocations);

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