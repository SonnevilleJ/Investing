using System;
using System.Linq;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.Investing.PortfolioManager.Configuration;
using Sonneville.Investing.PortfolioManager.FidelityWebDriver;
using Sonneville.Investing.Trading;

namespace Sonneville.Investing.PortfolioManager
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
        private readonly PortfolioManagerConfiguration _portfolioManagerConfiguration;

        public AccountRebalancer(IPositionsManager positionsManager,
            IAccountMapper accountMapper,
            ISecuritiesAllocationCalculator securitiesAllocationCalculator,
            PortfolioManagerConfiguration portfolioManagerConfiguration)
        {
            _positionsManager = positionsManager;
            _accountMapper = accountMapper;
            _securitiesAllocationCalculator = securitiesAllocationCalculator;
            _portfolioManagerConfiguration = portfolioManagerConfiguration;
        }

        public void RebalanceAccounts()
        {
            var accounts = _accountMapper.Map(_positionsManager.GetAccountDetails())
                .Where(account => _portfolioManagerConfiguration.InScopeAccountTypes.Contains(account.AccountType))
                .ToList();

            var allocations = _securitiesAllocationCalculator.CalculateAllocations(accounts);
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