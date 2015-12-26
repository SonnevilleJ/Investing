using System.Collections.Generic;
using Sonneville.Investing.Accounting.Cash;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Accounting.Securities
{
    public interface IShareAccount : ICashAccount
    {
        IReadOnlyCollection<IShareTransaction> ShareTransactions { get; }

        IShareAccount Buy(Buy buy);

        IShareAccount Sell(ISell sell);
    }
}