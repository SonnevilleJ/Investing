namespace Sonneville.Investing.Trading
{
    public enum TransactionType
    {
        Unknown,
        Deposit,
        Withdrawal,
        Buy,
        Sell,
        DividendReceipt,
        DividendReinvestment,
        SellShort,
        BuyToCover,
    }
}