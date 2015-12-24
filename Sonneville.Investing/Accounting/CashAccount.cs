using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;
using Sonneville.Investing.Accounting.Validation;

namespace Sonneville.Investing.Accounting
{
    public class CashAccount : ICashAccount
    {
        private readonly List<ICashTransaction> _transactions;

        private readonly ICashTransactionValidator<IDeposit> _depositValidator;

        private readonly ICashTransactionValidator<IWithdrawal> _withdrawalValidator;

        public CashAccount(ICashTransactionValidator<IDeposit> depositValidator, ICashTransactionValidator<IWithdrawal> withdrawalValidator)
        {
            _transactions = new List<ICashTransaction>();
            _depositValidator = depositValidator;
            _withdrawalValidator = withdrawalValidator;
        }

        public IReadOnlyCollection<ICashTransaction> CashTransactions => _transactions.AsReadOnly();

        public ICashAccount Deposit(IDeposit deposit)
        {
            _depositValidator.ThrowIfInvalid(deposit, _transactions);
            _transactions.Add(deposit);
            return this;
        }

        public ICashAccount Withdraw(IWithdrawal withdrawal)
        {
            _withdrawalValidator.ThrowIfInvalid(withdrawal, _transactions);
            _transactions.Add(withdrawal);
            return this;
        }
    }
}