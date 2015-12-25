using System.Collections.Generic;
using Sonneville.Investing.Accounting.Strategies;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public class ShareAccount : IShareAccount
    {
        private readonly ICashAccount _cashAccount;

        private readonly List<IShareTransaction> _shareTransactions;

        private readonly IBuyStrategy _buyStrategy;

        public ShareAccount(ICashAccount cashAccount, IBuyStrategy buyStrategy)
        {
            _cashAccount = cashAccount;
            _shareTransactions = new List<IShareTransaction>();
            _buyStrategy = buyStrategy;
        }

        public IReadOnlyCollection<ICashTransaction> CashTransactions => _cashAccount.CashTransactions;

        public IReadOnlyCollection<IShareTransaction> ShareTransactions => _shareTransactions.AsReadOnly();

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

        public IShareAccount Buy(Buy buy)
        {
            _buyStrategy.ProcessBuy(_cashAccount, buy);
            _shareTransactions.Add(buy);
            return this;
        }
    }
}