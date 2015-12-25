using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.ShareStrategies
{
    public interface IShareTransactionStrategy<in T> where T : IShareTransaction
    {
        void ProcessTransaction(IShareAccount shareAccount, T transaction);
    }
}