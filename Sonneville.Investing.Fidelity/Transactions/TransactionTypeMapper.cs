using System;
using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Domain;

namespace Sonneville.Investing.Fidelity.Transactions
{
    public interface ITransactionTypeMapper
    {
        TransactionType ClassifyDescription(string description);
    }

    public class TransactionTypeMapper : ITransactionTypeMapper
    {
        private readonly Dictionary<string, TransactionType> _reverseMappings = new Dictionary<string, TransactionType>
        {
            {"YOU BOUGHT", TransactionType.Buy},
            {"YOU SOLD", TransactionType.Sell},
            {"IN LIEU OF FRX SHARE", TransactionType.Sell}, // receipt of cash in lieu of fractional shares. Must compute cost basis. Earnings/(losses) are taxable.
            {"REINVESTMENT", TransactionType.DividendReinvestment},
            {"DIVIDEND RECEIVED", TransactionType.DividendReceipt},
            {"SHORT-TERM CAP GAIN", TransactionType.ShortTermCapGain},
            {"LONG-TERM CAP GAIN", TransactionType.LongTermCapGain},
            {"INTEREST EARNED", TransactionType.InterestEarned},
            {"Electronic Funds Transfer Received", TransactionType.Deposit},
            {"CASH CONTRIBUTION CURRENT YEAR", TransactionType.Deposit},
            {"DIRECT DEPOSIT ELAN CARDSVCRedemption (Cash)", TransactionType.Deposit},
            {"TO BROKERAGE OPTION", TransactionType.DepositBrokeragelink},
            {"PARTIC CONTR", TransactionType.DepositHSA},
            {"Electronic Funds Transfer Paid", TransactionType.Withdrawal},
        };

        public TransactionType ClassifyDescription(string description)
        {
            return _reverseMappings.SingleOrDefault(mapping => description.Contains(mapping.Key)).Value;
        }

        public string GetExampleDescription(TransactionType transactionType)
        {
            if (!_reverseMappings.Values.Contains(transactionType)) throw new NotImplementedException();
            return _reverseMappings.First(kvp => kvp.Value == transactionType).Key;
        }
    }
}
