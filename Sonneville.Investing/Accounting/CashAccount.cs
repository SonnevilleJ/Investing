using System.Collections.Generic;
using Sonneville.Investing.Accounting.CashStrategies;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public class CashAccount : ICashAccount
    {
        private readonly List<ICashTransaction> _transactions;

        private readonly ICashTransactionStrategy<IDeposit> _depositStrategy;

        private readonly ICashTransactionStrategy<IWithdrawal> _withdrawalStrategy;

        public CashAccount(ICashTransactionStrategy<IDeposit> depositStrategy, ICashTransactionStrategy<IWithdrawal> withdrawalStrategy)
        {
            _transactions = new List<ICashTransaction>();
            _depositStrategy = depositStrategy;
            _withdrawalStrategy = withdrawalStrategy;
        }

        public IReadOnlyCollection<ICashTransaction> CashTransactions => _transactions.AsReadOnly();

        public ICashAccount Deposit(IDeposit deposit)
        {
            _depositStrategy.ThrowIfInvalid(deposit, this);
            _transactions.Add(deposit);
            return this;
        }

        public ICashAccount Withdraw(IWithdrawal withdrawal)
        {
            _withdrawalStrategy.ThrowIfInvalid(withdrawal, this);
            _transactions.Add(withdrawal);
            return this;
        }
    }
}