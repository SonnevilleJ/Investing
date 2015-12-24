using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.Validation
{
    public interface ICashTransactionValidator<in T> where T : ICashTransaction
    {
        void ThrowIfInvalid(T transaction, IEnumerable<ICashTransaction> currentTransactions);
    }
}