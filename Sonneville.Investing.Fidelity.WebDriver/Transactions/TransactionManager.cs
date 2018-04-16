using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.WebDriver.Login;
using Sonneville.Investing.Fidelity.WebDriver.Navigation;

namespace Sonneville.Investing.Fidelity.WebDriver.Transactions
{
    public interface ITransactionManager : IManager
    {
        IList<ITransaction> GetTransactionHistory(DateTime startDate, DateTime endDate);
    }

    public class TransactionManager : ITransactionManager
    {
        private readonly ILog _log;
        private ISiteNavigator _siteNavigator;
        private ILoginManager _loginManager;

        public TransactionManager(ILog log, ISiteNavigator siteNavigator, ILoginManager loginManager)
        {
            _log = log;
            _siteNavigator = siteNavigator;
            _loginManager = loginManager;
        }

        public IList<ITransaction> GetTransactionHistory(DateTime startDate, DateTime endDate)
        {
            _log.Info("Getting transaction history...");

            _loginManager.EnsureLoggedIn();
            var activityPage = _siteNavigator.GoTo<IActivityPage>();
            return activityPage.GetTransactions(startDate, endDate).ToList();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _siteNavigator?.Dispose();
                _siteNavigator = null;

                _loginManager?.Dispose();
                _loginManager = null;
            }
        }
    }
}