using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.FidelityWebDriver;
using Sonneville.Fidelity.WebDriver.Data;
using FidelityPosition = Sonneville.Fidelity.WebDriver.Data.Position;
using Position = Sonneville.Investing.Trading.Position;

namespace Sonneville.Fidelity.Shell.Test.FidelityWebDriver
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
            var accountDetails = CreateAccountDetails("57", 100, AccountType.RetirementAccount);
            var mappedPositions = SetupPositionMapping(accountDetails);
            _positionMapperMock.Setup(mapper => mapper.Map(accountDetails.Positions)).Returns(mappedPositions);

            var mappedAccount = _accountMapper.Map(accountDetails);

            Assert.AreEqual(accountDetails.AccountNumber, mappedAccount.AccountId);
            Assert.AreEqual(accountDetails.PendingActivity, mappedAccount.PendingFunds);
            Assert.AreEqual(Investing.Trading.AccountType.RetirementAccount, mappedAccount.AccountType);
            CollectionAssert.AreEquivalent(mappedPositions, mappedAccount.Positions);
        }

        [Test]
        public void ShouldMapAccounts()
        {
            var accountDetails = new List<IAccountDetails>
            {
                CreateAccountDetails("57", 100, AccountType.RetirementAccount),
                CreateAccountDetails("58", 200, AccountType.HealthSavingsAccount),
                CreateAccountDetails("59", 300, AccountType.InvestmentAccount),
            };
            var positionMappings = SetupPositionMappings(accountDetails);

            var mappedAccounts = _accountMapper.Map(accountDetails).ToList();

            foreach (var accountDetail in accountDetails)
            {
                var mappedAccount = mappedAccounts.Single(account => account.AccountId == accountDetail.AccountNumber);
                Assert.AreEqual(accountDetail.AccountNumber, mappedAccount.AccountId);
                Assert.AreEqual(accountDetail.PendingActivity, mappedAccount.PendingFunds);
                CollectionAssert.AreEquivalent(positionMappings[accountDetail], mappedAccount.Positions);
            }
        }

        private IDictionary<IAccountDetails, List<Position>> SetupPositionMappings(
            IEnumerable<IAccountDetails> unmappedAccounts)
        {
            return unmappedAccounts.ToDictionary(account => account, SetupPositionMapping);
        }

        private List<Position> SetupPositionMapping(IAccountDetails unmappedAccount)
        {
            var mappedPositions = new List<Position>
            {
                new Position {Ticker = unmappedAccount.AccountNumber}
            };
            _positionMapperMock.Setup(mapper => mapper.Map(unmappedAccount.Positions)).Returns(mappedPositions);
            return mappedPositions;
        }

        private static AccountDetails CreateAccountDetails(string accountNumber, decimal pendingActivity,
            AccountType accountType)
        {
            var positions = new List<IPosition> {new FidelityPosition {Ticker = accountNumber}};
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