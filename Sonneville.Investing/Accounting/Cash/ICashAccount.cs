using System.Collections.Generic;
using Sonneville.Investing.Accounting.Cash.Transactions;

namespace Sonneville.Investing.Accounting.Cash
{
    public interface ICashAccount
    {
        IReadOnlyCollection<ICashTransaction> CashTransactions { get; }

        ICashAccount Deposit(IDeposit deposit);

        ICashAccount Withdraw(IWithdrawal withdrawal);
    }
}