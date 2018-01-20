using log4net;
using Sonneville.Fidelity.WebDriver.Configuration;
using Sonneville.Fidelity.WebDriver.Navigation;

namespace Sonneville.Fidelity.WebDriver.Login
{
    public interface ILoginManager : IManager
    {
        bool IsLoggedIn { get; }
        void EnsureLoggedIn();
    }

    public class LoginManager : ILoginManager
    {
        private readonly ILog _log;
        private ISiteNavigator _siteNavigator;
        private readonly FidelityConfiguration _fidelityConfiguration;

        public LoginManager(ILog log, ISiteNavigator siteNavigator, FidelityConfiguration fidelityConfiguration)
        {
            _log = log;
            _siteNavigator = siteNavigator;
            _fidelityConfiguration = fidelityConfiguration;
        }

        public bool IsLoggedIn { get; private set; }

        private void LogIn()
        {
            _log.Info("Logging in to Fidelity...");

            _siteNavigator.GoTo<ILoginPage>().LogIn(_fidelityConfiguration.Username, _fidelityConfiguration.Password);
            IsLoggedIn = true;
        }

        public void EnsureLoggedIn()
        {
            if (!IsLoggedIn)
            {
                LogIn();
            }
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
            }
        }
    }
}