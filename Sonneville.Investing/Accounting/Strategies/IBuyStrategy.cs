using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting.Strategies
{
    public interface IBuyStrategy
    {
        void ProcessBuy(ICashAccount cashAccount, Buy buy);
    }
}