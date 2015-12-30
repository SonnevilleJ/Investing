using System;
using System.Linq;
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
        private readonly ISecuritiesAllocationCalculator _securitiesAllocationCalculator;
        private readonly PositionMapper _positionMapper;

        public AccountRebalancer(IPositionsManager positionsManager, ISecuritiesAllocationCalculator securitiesAllocationCalculator)
        {
            _positionsManager = positionsManager;
            _securitiesAllocationCalculator = securitiesAllocationCalculator;
            _positionMapper = new PositionMapper();
        }

        public void RebalanceAccounts()
        {
            var positions = _positionsManager.GetAccountDetails()
                .SelectMany(account => account.Positions)
                .Select(position => _positionMapper.Map(position))
                .ToList();

            var allocations = _securitiesAllocationCalculator.CalculateAllocations(positions);
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