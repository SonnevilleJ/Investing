using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Domain;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Investing.Persistence
{
    public interface ITransactionRepository
    {
        void Save(IEnumerable<ITransaction> transactions);
        
        IEnumerable<ITransaction> Get();
    }

    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDataStore _dataStore;

        public TransactionRepository(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public void Save(IEnumerable<ITransaction> transactions)
        {
            var transactionData = new TransactionMule {Transactions = transactions.ToList()};
            _dataStore.Save(transactionData);
        }

        public IEnumerable<ITransaction> Get()
        {
            return _dataStore.Get<TransactionMule>().Transactions;
        }

        private class TransactionMule
        {
            public List<ITransaction> Transactions { get; set; }
        }
    }
}
