using System;
using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public class CashAccount : ICashAccount
    {
        private readonly List<ICashTransaction> _transactions;

        public CashAccount()
        {
            _transactions = new List<ICashTransaction>();
        }

        public IReadOnlyCollection<ICashTransaction> CashTransactions => _transactions.AsReadOnly();

        public ICashAccount Deposit(IDeposit deposit)
        {
            if (!DepositIsValid(deposit))
            {
                throw new InvalidOperationException("Cannot deposit a negative amount of funds!");
            }
            _transactions.Add(deposit);
            return this;
        }

        public ICashAccount Withdraw(IWithdrawal withdrawal)
        {
            if (!WithdrawalIsValid(withdrawal))
            {
                throw new InvalidOperationException("Cannot withdraw a positive amount of funds!");
            }
            _transactions.Add(withdrawal);
            return this;
        }

        private bool DepositIsValid(IDeposit deposit)
        {
            return deposit.Amount >= 0;
        }

        private bool WithdrawalIsValid(IWithdrawal withdrawal)
        {
            return withdrawal.Amount < 0;
        }
    }
}