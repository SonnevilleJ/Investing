using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.Investing.PortfolioManager.FidelityWebDriver;
using FidelityAccountType = Sonneville.FidelityWebDriver.Data.AccountType;
using Position = Sonneville.Investing.Trading.Position;

namespace Sonneville.Investing.PortfolioManager.Test.FidelityWebDriver
{
    [TestFixture]
    public class AccountMapperTests
    {
        private Mock<IPositionMapper> _positionMapperMock;
        private AccountMapper _accountMapper;

        [SetUp]
        public void Setup()
        {
            _positionMapperMock = new Mock<IPositionMapper>();

            _accountMapper = new AccountMapper(_positionMapperMock.Object);
        }

        [Test]
        public void ShouldMapAccount()
        {
            var accountDetails = CreateAccountDetails("57", 100, FidelityAccountType.RetirementAccount);
            var mappedPositions = new List<Position>();
            _positionMapperMock.Setup(mapper => mapper.Map(accountDetails.Positions)).Returns(mappedPositions);

            var mappedAccount = _accountMapper.Map(accountDetails);

            Assert.AreEqual(accountDetails.AccountNumber, mappedAccount.AccountId);
            Assert.AreEqual(accountDetails.PendingActivity, mappedAccount.PendingFunds);
            Assert.AreEqual(Trading.AccountType.RetirementAccount, mappedAccount.AccountType);
            Assert.AreSame(mappedPositions, mappedAccount.Positions);
        }

        [Test]
        public void ShouldMapAccounts()
        {
            var accountDetails = new List<IAccountDetails>
            {
                CreateAccountDetails("57", 100, FidelityAccountType.RetirementAccount),
                CreateAccountDetails("58", 200, FidelityAccountType.HealthSavingsAccount),
                CreateAccountDetails("59", 300, FidelityAccountType.InvestmentAccount),
            };

            var positionMappings = SetupPositionMappings(accountDetails);

            var mappedAccounts = _accountMapper.Map(accountDetails).ToList();

            foreach (var accountDetail in accountDetails)
            {
                var mappedAccount = mappedAccounts.Single(account => account.AccountId == accountDetail.AccountNumber);
                Assert.AreEqual(accountDetail.AccountNumber, mappedAccount.AccountId);
                Assert.AreEqual(accountDetail.PendingActivity, mappedAccount.PendingFunds);
                Assert.AreSame(positionMappings[accountDetail], mappedAccount.Positions);
            }
        }

        private IDictionary<IAccountDetails, IEnumerable<Position>> SetupPositionMappings(
            IEnumerable<IAccountDetails> unmappedAccounts)
        {
            return unmappedAccounts.ToDictionary(account => account, SetupPositionMapping);
        }

        private IEnumerable<Position> SetupPositionMapping(IAccountDetails unmappedAccount)
        {
            var mappedPositions = new List<Position>
            {
                new Position {Ticker = unmappedAccount.AccountNumber}
            };
            _positionMapperMock.Setup(mapper => mapper.Map(unmappedAccount.Positions)).Returns(mappedPositions);
            return mappedPositions;
        }

        private static AccountDetails CreateAccountDetails(string accountNumber, decimal pendingActivity, AccountType accountType)
        {
            var positions = new List<IPosition> {new Sonneville.FidelityWebDriver.Data.Position {Ticker = accountNumber}};
            return new AccountDetails
            {
                AccountNumber = accountNumber,
                PendingActivity = pendingActivity,
                Positions = positions,
                AccountType = accountType,
            };
        }
    }
}