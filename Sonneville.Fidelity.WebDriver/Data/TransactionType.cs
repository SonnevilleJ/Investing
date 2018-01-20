namespace Sonneville.Fidelity.WebDriver.Data
{
    public enum TransactionType
    {
        Unknown,
        Deposit,
        DepositBrokeragelink,
        DepositHSA,
        Withdrawal,
        Buy,
        Sell,
        InterestEarned,
        DividendReceipt,
        ShortTermCapGain,
        LongTermCapGain,
        DividendReinvestment,
        SellShort,
        BuyToCover,
    }
}