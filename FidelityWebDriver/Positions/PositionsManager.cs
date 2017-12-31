using System.Collections.Generic;
using log4net;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Login;
using Sonneville.FidelityWebDriver.Navigation;

namespace Sonneville.FidelityWebDriver.Positions
{
    public interface IPositionsManager : IManager
    {
        IEnumerable<IAccountSummary> GetAccountSummaries();

        IEnumerable<IAccountDetails> GetAccountDetails();
    }

    public class PositionsManager : IPositionsManager
    {
        private readonly ILog _log;
        private ISiteNavigator _siteNavigator;
        private ILoginManager _loginManager;

        public PositionsManager(ILog log, ISiteNavigator siteNavigator, ILoginManager loginManager)
        {
            _log = log;
            _siteNavigator = siteNavigator;
            _loginManager = loginManager;
        }

        public IEnumerable<IAccountSummary> GetAccountSummaries()
        {
            _log.Info("Getting account summaries...");
            _loginManager.EnsureLoggedIn();

            return _siteNavigator.GoTo<IPositionsPage>().GetAccountSummaries();
        }

        public IEnumerable<IAccountDetails> GetAccountDetails()
        {
            _log.Info("Getting account details...");
            _loginManager.EnsureLoggedIn();

            return _siteNavigator.GoTo<IPositionsPage>().GetAccountDetails();
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
