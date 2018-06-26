using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.WebDriver.Configuration;
using Sonneville.Investing.Fidelity.WebDriver.Positions;
using Sonneville.Investing.Fidelity.WebDriver.Transactions;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    public class DemoCommandTests : BaseCommandTests<DemoCommand>
    {
        private FidelityConfiguration _fidelityConfiguration;
        private string _username;
        private string _password;

        private Mock<ILog> _logMock;
        private Mock<IPositionsManager> _positionsManagerMock;
        private Mock<ITransactionManager> _transactionManagerMock;
        private List<ITransaction> _transactions;
        private List<AccountSummary> _accountSummaries;
        private List<AccountDetails> _accountDetails;
        private DateTime _startDate;
        private DateTime _endDate;

        private Mock<IDataStore> _dataStoreMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _username = "Batman";
            _password = "I am vengeance. I am the night. I am Batman.";

            _startDate = DateTime.Today.AddDays(-30);
            _endDate = DateTime.Today;

            _accountSummaries = new List<AccountSummary>
            {
                new AccountSummary
                {
                    AccountNumber = "acct 1",
                    AccountType = AccountType.InvestmentAccount,
                    Name = "play money",
                    MostRecentValue = 5000
                },
                new AccountSummary
                {
                    AccountNumber = "acct 2",
                    AccountType = AccountType.RetirementAccount,
                    Name = "play money",
                    MostRecentValue = 88176
                },
                new AccountSummary
                {
                    AccountNumber = "acct 3",
                    AccountType = AccountType.HealthSavingsAccount,
                    Name = "don't get sick",
                    MostRecentValue = 1800
                },
                new AccountSummary
                {
                    AccountNumber = "acct 4",
                    AccountType = AccountType.CreditCard,
                    Name = "debt",
                    MostRecentValue = 1200
                },
            };

            _accountDetails = new List<AccountDetails>
            {
                new AccountDetails
                {
                    Name = "first account",
                    AccountNumber = "acct a",
                    Positions = new List<IPosition>
                    {
                        new Position
                        {
                            Ticker = "asdf",
                            Quantity = 2,
                            CostBasisPerShare = 5.27m,
                            CurrentValue = 10.54m,
                        },
                        new Position
                        {
                            Ticker = "querty",
                            Quantity = 35,
                            CostBasisPerShare = 8.49m,
                            CurrentValue = 297.15m,
                        },
                    }
                },
                new AccountDetails
                {
                    Name = "second account",
                    AccountNumber = "acct b",
                    Positions = new List<IPosition>
                    {
                        new Position
                        {
                            Ticker = "aapl",
                            Quantity = 2,
                            CostBasisPerShare = 195.27m,
                            CurrentValue = 390.54m,
                        },
                        new Position
                        {
                            Ticker = "msft",
                            Quantity = 35,
                            CostBasisPerShare = 1.04m,
                            CurrentValue = 36.4m,
                        },
                    }
                },
            };

            _transactions = new List<ITransaction>
            {
                new Transaction
                {
                    RunDate = new DateTime(2015, 12, 25),
                    AccountNumber = null,
                    Action = null,
                    Type = TransactionType.Buy,
                    Symbol = "DE",
                    SecurityDescription = null,
                    SecurityType = null,
                    Quantity = 12.3m,
                    Price = 45.67m,
                    Commission = 8.9m,
                    Fees = 0.0m,
                    AccruedInterest = 0.0m,
                    Amount = 0.0m,
                    SettlementDate = new DateTime(2015, 12, 31),
                }
            };

            _positionsManagerMock = new Mock<IPositionsManager>();
            _positionsManagerMock.Setup(manager => manager.GetAccountSummaries()).Returns(_accountSummaries);
            _positionsManagerMock.Setup(manager => manager.GetAccountDetails()).Returns(_accountDetails);

            _transactionManagerMock = new Mock<ITransactionManager>();
            _transactionManagerMock.Setup(manager => manager.GetTransactionHistory(_startDate, _endDate))
                .Returns(_transactions);

            _fidelityConfiguration = new FidelityConfiguration();

            _logMock = new Mock<ILog>();

            _dataStoreMock = new Mock<IDataStore>();
            _dataStoreMock.Setup(configStore => configStore.Load<FidelityConfiguration>())
                .Returns(_fidelityConfiguration);
            _dataStoreMock.Setup(configStore => configStore.Save(It.IsAny<FidelityConfiguration>()))
                .Callback<FidelityConfiguration>(config => _fidelityConfiguration = config);

            Command = new DemoCommand(
                _logMock.Object,
                _dataStoreMock.Object,
                _positionsManagerMock.Object,
                _transactionManagerMock.Object,
                new TransactionTranslator());
        }

        [Test]
        public override void HasCorrectTitle()
        {
            Assert.AreEqual("demo", Command.CommandName);
        }

        [Test]
        public override void ShouldDisposeOfDependencies()
        {
            Command.Dispose();

            _positionsManagerMock.Verify(manager => manager.Dispose());
            _transactionManagerMock.Verify(manager => manager.Dispose());
        }

        [Test]
        public void ShouldFetchAccountSummariesFromPositionsManager()
        {
            CacheCredentials(_username, _password);
            
            var shouldExit = InvokeCommand();

            Assert.IsFalse(shouldExit);
            _accountSummaries.ForEach(AssertOutputContainsAccountSummary);
        }

        [Test]
        public void ShouldFetchAccountDetailsFromPositionsManager()
        {
            CacheCredentials(_username, _password);
            
            var shouldExit = InvokeCommand();

            Assert.IsFalse(shouldExit);
            _accountDetails.ForEach(AssertOutputContainsAccountDetails);
        }

        [Test]
        public void ShouldGetTransactionHistoryFromTransactionsManager()
        {
            CacheCredentials(_username, _password);
            
            var shouldExit = InvokeCommand();

            Assert.IsFalse(shouldExit);
            _transactions.ForEach(AssertOutputContainsTransactionDetails);
        }

        [Test]
        public void ShouldPrintCompleteMessage()
        {
            CacheCredentials(_username, _password);

            var shouldExit = InvokeCommand();

            Assert.IsFalse(shouldExit);
            AssertOutputContains("Demo completed");
        }

        [Test]
        public void ShouldSetConfigFromCliArgsWithoutPersisting()
        {
            var args = new[] {"-u", _username, "-p", _password};

            var shouldExit = InvokeCommand(args);

            Assert.IsFalse(shouldExit);
            Assert.AreEqual(_username, _fidelityConfiguration.Username);
            Assert.AreEqual(_password, _fidelityConfiguration.Password);
            AssertUnchangedConfig();
        }

        [Test]
        public void ShouldSetConfigFromCliArgsAndPersist()
        {
            _dataStoreMock.Setup(configStore => configStore.Save(It.IsAny<FidelityConfiguration>()))
                .Callback<FidelityConfiguration>(config =>
                {
                    Assert.AreEqual(_username, config.Username);
                    Assert.AreEqual(_password, config.Password);
                });
            var args = new[] {"-u", _username, "-p", _password, "-s"};

            var shouldExit = InvokeCommand(args);

            Assert.IsFalse(shouldExit);
            _logMock.Verify(log =>
                log.Info(It.Is<string>(message => message.Contains($"Saving credentials for `{_username}`."))));
            _dataStoreMock.Verify(configStore => configStore.Save(_fidelityConfiguration));
        }

        [Test]
        public void ShouldDisplayHelpFromCliArgsAndNotPersist()
        {
            var args = new[] {"-u", _username, "-p", _password, "-s", "-h"};
            
            var shouldExit = InvokeCommand(args);

            Assert.IsFalse(shouldExit);
            AssertOutputContains("-h");
            AssertUnchangedConfig();
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("user1234", null)]
        public void ShouldPromptForCredentialsWhenCredentialsAreInsufficientlyCached(string username, string password)
        {
            CacheCredentials(username, password);
            EnqueueInput(_username, _password);

            var shouldExit = InvokeCommand();

            Assert.IsFalse(shouldExit);
            Assert.AreEqual(_username, _fidelityConfiguration.Username);
            Assert.AreEqual(_password, _fidelityConfiguration.Password);
        }

        [Test]
        public void ShouldLogWhenCredentialsAreCached()
        {
            CacheCredentials(_username, _password);

            var shouldExit = InvokeCommand();

            Assert.IsFalse(shouldExit);
            _logMock.Verify(log => log.Info(It.Is<string>(message => message.Contains(_username))));
            Assert.AreEqual(_username, _fidelityConfiguration.Username);
            Assert.AreEqual(_password, _fidelityConfiguration.Password);
        }

        private void CacheCredentials(string username, string password)
        {
            _fidelityConfiguration.Username = username;
            _fidelityConfiguration.Password = password;
        }

        private void AssertOutputContainsAccountSummary(AccountSummary account)
        {
            AssertOutputContains(account.AccountNumber);
            AssertOutputContains(account.Name);
            AssertOutputContains(account.MostRecentValue.ToString("C"));
        }

        private void AssertOutputContainsAccountDetails(AccountDetails account)
        {
            AssertOutputContains(account.Name);
            AssertOutputContains(account.AccountNumber);
            AssertOutputContains(account.AccountType.ToString());
            account.Positions.ToList().ForEach(AssertOutputContainsPositionDetails);
        }

        private void AssertOutputContainsPositionDetails(IPosition position)
        {
            AssertOutputContains(position.Ticker);
            AssertOutputContains(position.Quantity.ToString("N"));
            AssertOutputContains(position.CurrentValue.ToString("C"));
            AssertOutputContains(position.CostBasisPerShare.ToString("C"));
        }

        private void AssertOutputContainsTransactionDetails(ITransaction transaction)
        {
            AssertOutputContains(transaction.RunDate.Value.ToString("d"));
            AssertOutputContains(transaction.Quantity.Value.ToString("F"));
            AssertOutputContains(transaction.Symbol);
            AssertOutputContains(transaction.Price.Value.ToString("C"));
        }

        private void AssertUnchangedConfig()
        {
            _dataStoreMock.Verify(configStore => configStore.Save(It.IsAny<object>()), Times.Never);
        }
    }
}
