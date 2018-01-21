using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;
using Sonneville.Fidelity.WebDriver.Configuration;
using Sonneville.Fidelity.WebDriver.Data;
using Sonneville.Fidelity.WebDriver.Positions;
using Sonneville.Fidelity.WebDriver.Transactions;
using Sonneville.Utilities.Persistence.v1;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    public class DemoCommandTests
    {
        private FidelityConfiguration _fidelityConfiguration;
        private string _username;
        private string _password;

        private Mock<ILog> _logMock;
        private Mock<IPositionsManager> _positionsManagerMock;
        private Mock<ITransactionManager> _transactionManagerMock;
        private List<IFidelityTransaction> _transactions;
        private List<AccountSummary> _accountSummaries;
        private List<AccountDetails> _accountDetails;
        private DateTime _startDate;
        private DateTime _endDate;

        private Mock<IDataStore> _dataStoreMock;

        private MemoryStream _inputStream;
        private StreamReader _inputReader;
        private StreamWriter _inputWriter;
        private MemoryStream _outputStream;
        private StreamReader _outputReader;
        private StreamWriter _outputWriter;

        private DemoCommand _command;

        [SetUp]
        public void Setup()
        {
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

            _transactions = new List<IFidelityTransaction>
            {
                new FidelityTransaction
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
            _dataStoreMock.Setup(configStore => configStore.Load<FidelityConfiguration>()).Returns(_fidelityConfiguration);
            _dataStoreMock.Setup(configStore => configStore.Save(It.IsAny<FidelityConfiguration>())).Callback<FidelityConfiguration>(config => _fidelityConfiguration = config);

            _inputStream = new MemoryStream();
            _inputReader = new StreamReader(_inputStream);
            _inputWriter = new StreamWriter(_inputStream) {AutoFlush = true};

            _outputStream = new MemoryStream();
            _outputReader = new StreamReader(_outputStream);
            _outputWriter = new StreamWriter(_outputStream) {AutoFlush = true};

            _command = new DemoCommand(
                _logMock.Object,
                _dataStoreMock.Object,
                _positionsManagerMock.Object,
                _transactionManagerMock.Object,
                new TransactionTranslator());
        }

        [TearDown]
        public void Teardown()
        {
            _inputStream.Dispose();
            _inputReader.Dispose();
            _inputWriter.Dispose();

            _outputStream.Dispose();
            _outputReader.Dispose();
            _outputWriter.Dispose();

            _command?.Dispose();
        }

        [Test]
        public void HasCorrectTitle()
        {
            Assert.AreEqual("demo", _command.CommandName);
        }

        [Test]
        public void ShouldExitAfter()
        {
            Assert.IsFalse(_command.ExitAfter);
        }

        [Test]
        public void ShouldFetchAccountSummariesFromPositionsManager()
        {
            CacheCredentials(_username, _password);
            _command.Invoke(_inputReader, _outputWriter, new string[0]);

            var outputText = ReadOutputText();
            _accountSummaries.ForEach(account =>
            {
                Assert.IsTrue(outputText.Contains(account.AccountNumber));
                Assert.IsTrue(outputText.Contains(account.Name));
                Assert.IsTrue(outputText.Contains(account.MostRecentValue.ToString("C")));
            });
        }

        [Test]
        public void ShouldFetchAccountDetailsFromPositionsManager()
        {
            CacheCredentials(_username, _password);
            _command.Invoke(_inputReader, _outputWriter, new string[0]);

            var outputText = ReadOutputText();
            _accountDetails.ForEach(account =>
            {
                Assert.IsTrue(outputText.Contains(account.Name));
                Assert.IsTrue(outputText.Contains(account.AccountNumber));
                Assert.IsTrue(outputText.Contains(account.AccountType.ToString()));
                account.Positions.ToList()
                    .ForEach(position =>
                    {
                        Assert.IsTrue(outputText.Contains(position.Ticker));
                        Assert.IsTrue(outputText.Contains(position.Quantity.ToString("N")));
                        Assert.IsTrue(outputText.Contains(position.CurrentValue.ToString("C")));
                        Assert.IsTrue(outputText.Contains(position.CostBasisPerShare.ToString("C")));
                    });
            });
        }

        [Test]
        public void ShouldGetTransactionHistoryFromTransactionsManager()
        {
            CacheCredentials(_username, _password);
            _command.Invoke(_inputReader, _outputWriter, new string[0]);

            var outputText = ReadOutputText();
            _transactions.ForEach(transaction =>
            {
                Assert.IsTrue(outputText.Contains(transaction.RunDate.Value.ToString("d")));
                Assert.IsTrue(outputText.Contains(transaction.Quantity.Value.ToString("F")));
                Assert.IsTrue(outputText.Contains(transaction.Symbol));
                Assert.IsTrue(outputText.Contains(transaction.Price.Value.ToString("C")));
            });

            _transactionManagerMock.Verify(manager => manager.GetTransactionHistory(_startDate, _endDate));
        }

        [Test]
        public void ShouldPrintCompleteMessage()
        {
            CacheCredentials(_username, _password);
            _command.Invoke(_inputReader, _outputWriter, new string[0]);

            var outputText = ReadOutputText();
            Assert.IsTrue(outputText.Contains("Demo completed"));
        }

        [Test]
        public void ShouldSetConfigFromCliArgsWithoutPersisting()
        {
            var args = new[] {"-u", _username, "-p", _password};

            _command.Invoke(_inputReader, _outputWriter, args);

            Assert.AreEqual(_username, _fidelityConfiguration.Username);
            Assert.AreEqual(_password, _fidelityConfiguration.Password);
            AssertUnchangedConfig();
        }

        [Test]
        public void ShouldSetConfigFromCliArgsAndPersist()
        {
            _dataStoreMock.Setup(configStore => configStore.Save(It.IsAny<FidelityConfiguration>())).Callback<FidelityConfiguration>(config =>
            {
                Assert.AreEqual(_username, config.Username);
                Assert.AreEqual(_password, config.Password);
            });
            var args = new[] {"-u", _username, "-p", _password, "-s"};

            _command.Invoke(_inputReader, _outputWriter, args);

            _logMock.Verify(log => log.Info(It.Is<string>(message => message.Contains($"Saving credentials for `{_username}`."))));
            _dataStoreMock.Verify(configStore => configStore.Save(_fidelityConfiguration));
        }

        [Test]
        public void ShouldDisplayHelpFromCliArgsAndNotPersist()
        {
            _command.Invoke(_inputReader, _outputWriter, new[] {"-u", _username, "-p", _password, "-s", "-h"});

            var outputText = ReadOutputText();
            Assert.IsTrue(outputText.Contains("-h"),
                $"Actual console output follows:{Environment.NewLine}{outputText}");
            AssertUnchangedConfig();
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("user1234", null)]
        public void ShouldPromptForCredentialsWhenCredentialsAreInsufficientlyCached(string username, string password)
        {
            CacheCredentials(username, password);

            _inputWriter.WriteLine(_username);
            _inputWriter.WriteLine(_password);
            var endlineLength = Environment.NewLine.Length;
            _inputStream.Position -= _username.Length + endlineLength + _password.Length + endlineLength;

            _command.Invoke(_inputReader, _outputWriter, new string[] { });

            Assert.AreEqual(_username, _fidelityConfiguration.Username);
            Assert.AreEqual(_password, _fidelityConfiguration.Password);
        }

        [Test]
        public void ShouldLogWhenCredentialsAreCached()
        {
            CacheCredentials(_username, _password);
            _command.Invoke(_inputReader, _outputWriter, new string[] { });

            _logMock.Verify(log => log.Info(It.Is<string>(message => message.Contains(_username))));
            Assert.AreEqual(_username, _fidelityConfiguration.Username);
            Assert.AreEqual(_password, _fidelityConfiguration.Password);
        }

        [Test]
        public void ShouldCascadeDisposeToManagers()
        {
            _command.Dispose();

            _positionsManagerMock.Verify(manager => manager.Dispose());
            _transactionManagerMock.Verify(manager => manager.Dispose());
        }

        [Test]
        public void ShouldHandleMultipleDisposals()
        {
            _command.Dispose();
            _command.Dispose();
        }

        private void CacheCredentials(string username, string password)
        {
            _fidelityConfiguration.Username = username;
            _fidelityConfiguration.Password = password;
        }

        private void AssertUnchangedConfig()
        {
            _dataStoreMock.Verify(configStore => configStore.Save(It.IsAny<object>()), Times.Never);
        }

        private string ReadOutputText()
        {
            _outputStream.Position = 0;
            return _outputReader.ReadToEnd();
        }
    }
}
