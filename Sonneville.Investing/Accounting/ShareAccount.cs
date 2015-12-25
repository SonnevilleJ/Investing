using System.Collections.Generic;
using System.Linq;
using Sonneville.Investing.Accounting.ShareStrategies;
using Sonneville.Investing.Accounting.Transactions;

namespace Sonneville.Investing.Accounting
{
    public class ShareAccount : IShareAccount
    {
        private readonly ICashAccount _cashAccount;

        private readonly List<IShareTransaction> _shareTransactions;

        private readonly IShareTransactionStrategy<Buy> _buyStrategy;

        private readonly IShareTransactionStrategy<Sell> _sellStrategy;

        public ShareAccount(ICashAccount cashAccount, IShareTransactionStrategy<Buy> buyStrategy, IShareTransactionStrategy<Sell> sellStrategy)
        {
            _shareTransactions = new List<IShareTransaction>();
            _cashAccount = cashAccount;
            _buyStrategy = buyStrategy;
            _sellStrategy = sellStrategy;
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
            _buyStrategy.ProcessTransaction(this, buy);
            _shareTransactions.Add(buy);
            return this;
        }

        public IShareAccount Sell(Sell sell)
        {
            _sellStrategy.ProcessTransaction(this, sell);
            _shareTransactions.Add(sell);
            return this;
        }

        public decimal CountHeldShares(string ticker)
        {
            return ShareTransactions
                .Where(transaction => transaction.Ticker == ticker)
                .Sum(transaction => transaction.Shares);
        }
    }
}