using System;
using System.Linq;
using Sonneville.Fidelity.Shell.FidelityWebDriver;
using Sonneville.Investing.Fidelity.WebDriver.Configuration;
using Sonneville.Investing.Fidelity.WebDriver.Positions;
using Sonneville.Investing.Trading;

namespace Sonneville.Fidelity.Shell
{
    public interface IAccountRebalancer : IDisposable
    {
        void RebalanceAccounts();
    }

    public class AccountRebalancer : IAccountRebalancer
    {
        private readonly IPositionsManager _positionsManager;
        private readonly IAccountMapper _accountMapper;
        private readonly ISecuritiesAllocationCalculator _securitiesAllocationCalculator;
        private readonly FidelityConfiguration _seleniumConfiguration;

        public AccountRebalancer(IPositionsManager positionsManager,
            IAccountMapper accountMapper,
            ISecuritiesAllocationCalculator securitiesAllocationCalculator,
            FidelityConfiguration seleniumConfiguration)
        {
            _positionsManager = positionsManager;
            _accountMapper = accountMapper;
            _securitiesAllocationCalculator = securitiesAllocationCalculator;
            _seleniumConfiguration = seleniumConfiguration;
        }

        public void RebalanceAccounts()
        {
            var accounts = _positionsManager.GetAccountDetails()
                .Where(account => _seleniumConfiguration.InScopeAccountTypes.Contains(account.AccountType))
                .Select(_accountMapper.Map)
                .ToList();

            var allocations = _securitiesAllocationCalculator.CalculateAccountAllocation(accounts);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _positionsManager?.Dispose();
            }
        }
    }
}