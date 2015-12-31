using System;
using Sonneville.FidelityWebDriver.Positions;
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

        public AccountRebalancer(IPositionsManager positionsManager, IAccountMapper accountMapper,
            ISecuritiesAllocationCalculator securitiesAllocationCalculator)
        {
            _positionsManager = positionsManager;
            _accountMapper = accountMapper;
            _securitiesAllocationCalculator = securitiesAllocationCalculator;
        }

        public void RebalanceAccounts()
        {
            var accounts = _accountMapper.Map(_positionsManager.GetAccountDetails());

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