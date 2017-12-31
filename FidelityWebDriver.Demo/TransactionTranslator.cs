using Sonneville.FidelityWebDriver.Data;

namespace Sonneville.FidelityWebDriver.Demo
{
    public class TransactionTranslator
    {
        public string Translate(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.Deposit:
                    return "deposited";
                case TransactionType.Withdrawal:
                    return "withdrew";
                case TransactionType.Buy:
                    return "bought";
                case TransactionType.Sell:
                    return "sold";
                case TransactionType.BuyToCover:
                    return "bought to cover short position";
                case TransactionType.SellShort:
                    return "sold short";
                case TransactionType.DividendReceipt:
                    return "received";
                case TransactionType.DividendReinvestment:
                    return "reinvested";
                default:
                    return $"Unknown transaction type: {transactionType}";
            }
        }
    }
}