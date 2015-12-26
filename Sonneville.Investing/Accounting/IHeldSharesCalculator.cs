using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public interface IHeldSharesCalculator
    {
        decimal CountHeldShares(string ticker, IEnumerable<IShareTransaction> shareTransactions);
    }
}