using System.Collections.Generic;
using Sonneville.Investing.Accounting.Securities.Transactions;

namespace Sonneville.Investing.Accounting.Securities
{
    public interface IHeldSharesCalculator
    {
        decimal CountHeldShares(string ticker, IEnumerable<IShareTransaction> shareTransactions);
    }
}