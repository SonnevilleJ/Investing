namespace Sonneville.Investing.Accounting.Transactions
{
    public interface IShareTransaction : ICashTransaction
    {
        string Ticker { get; }
        decimal Shares { get; }
        decimal PerSharePrice { get; }
        decimal Commission { get; }
    }
}