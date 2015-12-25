using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public interface IShareAccount : ICashAccount
    {
        IReadOnlyCollection<IShareTransaction> ShareTransactions { get; }

        IShareAccount Buy(Buy buy);

        IShareAccount Sell(ISell sell);

        decimal CountHeldShares(string ticker);
    }
}