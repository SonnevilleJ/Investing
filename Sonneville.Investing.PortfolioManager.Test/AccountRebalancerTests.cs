using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.Investing.Trading;
using Position = Sonneville.Investing.Trading.Position;

namespace Sonneville.Investing.PortfolioManager.Test
{
    [TestFixture]
    public class AccountRebalancerTests
    {
        private AccountRebalancer _accountRebalancer;
        private Mock<IPositionsManager> _positionsManagerMock;
        private Mock<ISecuritiesAllocationCalculator> _allocationCalculatorMock;

        [SetUp]
        public void Setup()
        {
            var accountDetails = new List<IAccountDetails>
            {
                new AccountDetails
                {
                    Positions = new List<IPosition>()
                }
            };

            _positionsManagerMock = new Mock<IPositionsManager>();
            _positionsManagerMock.Setup(manager => manager.GetAccountDetails()).Returns(accountDetails);

            _allocationCalculatorMock = new Mock<ISecuritiesAllocationCalculator>();

            _accountRebalancer = new AccountRebalancer(_positionsManagerMock.Object, _allocationCalculatorMock.Object);
        }

        [Test]
        public void DisposeShouldNotThrow()
        {
            _accountRebalancer.Dispose();
            _accountRebalancer.Dispose();

            _positionsManagerMock.Verify(manager => manager.Dispose());
        }

        [Test]
        public void RunShouldNotThrow()
        {
            _accountRebalancer.RebalanceAccounts();

            _allocationCalculatorMock.Verify(calculator => calculator.CalculateAllocations(It.IsAny<IReadOnlyList<Position>>()));
        }
    }
}