using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public class ShareAccount : ICashAccount
    {
        private readonly ICashAccount _cashAccount;

        public ShareAccount(ICashAccount cashAccount)
        {
            _cashAccount = cashAccount;
        }

        public IReadOnlyCollection<ICashTransaction> CashTransactions => _cashAccount.CashTransactions;

        public ICashAccount Deposit(IDeposit deposit)
        {
            _cashAccount.Deposit(deposit);
            return this;
        }

        public ICashAccount Withdraw(IWithdrawal withdrawal)
        {
            _cashAccount.Withdraw(withdrawal);
            return this;
        }
    }
}