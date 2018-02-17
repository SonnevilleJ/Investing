using System.Collections.Generic;
using System.Linq;
using Sonneville.Fidelity.WebDriver.Data;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Investing.Persistence
{
    public interface ITransactionRepository
    {
        void Save(IEnumerable<IFidelityTransaction> transactions);
        
        IEnumerable<IFidelityTransaction> Get();
    }

    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDataStore _dataStore;

        public TransactionRepository(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public void Save(IEnumerable<IFidelityTransaction> transactions)
        {
            var transactionData = new TransactionMule {Transactions = transactions.ToList()};
            _dataStore.Save(transactionData);
        }

        public IEnumerable<IFidelityTransaction> Get()
        {
            return _dataStore.Get<TransactionMule>().Transactions;
        }

        private class TransactionMule
        {
            public List<IFidelityTransaction> Transactions { get; set; }
        }
    }
}
