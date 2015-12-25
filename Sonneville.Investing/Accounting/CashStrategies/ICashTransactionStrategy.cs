using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.CashStrategies
{
    public interface ICashTransactionStrategy<in T> where T : ICashTransaction
    {
        void ThrowIfInvalid(T transaction, ICashAccount cashAccount);
    }
}