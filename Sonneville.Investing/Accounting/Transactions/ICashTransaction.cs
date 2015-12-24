using System;

namespace Sonneville.Investing.Accounting.Transactions
{
    public interface ICashTransaction
    {
        DateTime SettlementDate { get; }
        decimal Amount { get; }
        string Memo { get; }
    }
}