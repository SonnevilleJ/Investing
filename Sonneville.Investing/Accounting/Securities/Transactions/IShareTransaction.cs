using Sonneville.Investing.Accounting.Cash.Transactions;

namespace Sonneville.Investing.Accounting.Securities.Transactions
{
    public interface IShareTransaction : ICashTransaction
    {
        string Ticker { get; }
        decimal Shares { get; }
        decimal PerSharePrice { get; }
        decimal Commission { get; }
    }
}