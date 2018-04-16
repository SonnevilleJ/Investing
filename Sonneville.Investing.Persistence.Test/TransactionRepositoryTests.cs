using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.Investing.Domain;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Investing.Persistence.Test
{
    [TestFixture]
    public class TransactionRepositoryTests
    {
        private ITransactionRepository _repository;
        private List<ITransaction> _fidelityTransactions;
        private IDataStore _dataStore;

        [SetUp]
        public void Setup()
        {
            _fidelityTransactions = new List<ITransaction>
            {
                new Transaction(),
            };

            _repository = InitializeNewRepository();
        }

        [TearDown]
        public void Teardown()
        {
            _dataStore?.DeleteAll();
        }

        [Test]
        public void ShouldPersistTransactions()
        {
            _repository.Save(_fidelityTransactions);

            Assert.AreEqual(1, _dataStore.CachedCount);
        }

        [Test]
        public void ShouldRetrieveTransactions()
        {
            _repository.Save(_fidelityTransactions);

            var transactions = _repository.Get();

            CollectionAssert.AreEquivalent(_fidelityTransactions, transactions);
        }

        private TransactionRepository InitializeNewRepository()
        {
            _dataStore = new InMemoryDataStore();

            return new TransactionRepository(_dataStore);
        }
    }
}
