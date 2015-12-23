using System;
using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public class CashAccount
    {
        private readonly List<ICashTransaction> _transactions;

        public CashAccount()
        {
            _transactions = new List<ICashTransaction>();
        }

        public IReadOnlyCollection<ICashTransaction> Transactions => _transactions.AsReadOnly();

        public CashAccount Deposit(Deposit deposit)
        {
            if (!DepositIsValid(deposit))
            {
                throw new InvalidOperationException("Cannot deposit a negative amount of funds!");
            }
            _transactions.Add(deposit);
            return this;
        }

        public CashAccount Withdraw(Withdrawal withdrawal)
        {
            if (!WithdrawalIsValid(withdrawal))
            {
                throw new InvalidOperationException("Cannot withdraw a positive amount of funds!");
            }
            _transactions.Add(withdrawal);
            return this;
        }

        public decimal CalculateBalance(DateTime dateTime)
        {
            return _transactions
                .Where(transaction => transaction.SettlementDate <= dateTime)
                .Sum(transaction => transaction.Amount);
        }

        private bool DepositIsValid(Deposit deposit)
        {
            return deposit.Amount >= 0;
        }

        private bool WithdrawalIsValid(Withdrawal withdrawal)
        {
            return withdrawal.Amount < 0;
        }
    }
}