using System.Collections.Generic;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public interface ICashAccount
    {
        IReadOnlyCollection<ICashTransaction> CashTransactions { get; }

        ICashAccount Deposit(IDeposit deposit);

        ICashAccount Withdraw(IWithdrawal withdrawal);
    }
}