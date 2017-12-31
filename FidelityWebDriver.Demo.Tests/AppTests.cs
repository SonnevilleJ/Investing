using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Demo.Tests.Ninject;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.FidelityWebDriver.Transactions;

namespace Sonneville.FidelityWebDriver.Demo.Tests
{
    [TestFixture]
    public class AppTests
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

        private App _app;

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

            _accountDetails = new List<AccountDetails>()
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

            _app = new App(
                _logMock.Object,
                _positionsManagerMock.Object,
                _transactionManagerMock.Object,
                _fidelityConfiguration,
                new TransactionTranslator());
        }

        [TearDown]
        public void Teardown()
        {
            FidelityConfigurationProviderTests.DeletePersistedConfig();

            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true});
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));

            _app?.Dispose();
        }

        [Test]
        public void ShouldFetchAccountSummariesFromPositionsManager()
        {
            SetCredentials();
            using (var memoryStream = new MemoryStream())
            {
                RedirectConsoleOutput(memoryStream);

                _app.Run(new string[0]);

                var consoleOutput = ReadConsoleOutput(memoryStream);
                _accountSummaries.ForEach(account =>
                {
                    Assert.IsTrue(consoleOutput.Contains(account.AccountNumber));
                    Assert.IsTrue(consoleOutput.Contains(account.Name));
                    Assert.IsTrue(consoleOutput.Contains(account.MostRecentValue.ToString("C")));
                });
            }
        }

        [Test]
        public void ShouldFetchAccountDetailsFromPositionsManager()
        {
            SetCredentials();
            using (var memoryStream = new MemoryStream())
            {
                RedirectConsoleOutput(memoryStream);

                _app.Run(new string[0]);

                var consoleOutput = ReadConsoleOutput(memoryStream);
                _accountDetails.ForEach(account =>
                {
                    Assert.IsTrue(consoleOutput.Contains(account.Name));
                    Assert.IsTrue(consoleOutput.Contains(account.AccountNumber));
                    Assert.IsTrue(consoleOutput.Contains(account.AccountType.ToString()));
                    account.Positions.ToList()
                        .ForEach(position =>
                        {
                            Assert.IsTrue(consoleOutput.Contains(position.Ticker));
                            Assert.IsTrue(consoleOutput.Contains(position.Quantity.ToString("N")));
                            Assert.IsTrue(consoleOutput.Contains(position.CurrentValue.ToString("C")));
                            Assert.IsTrue(consoleOutput.Contains(position.CostBasisPerShare.ToString("C")));
                        });
                });
            }
        }

        [Test]
        public void ShouldGetTransactionHistoryFromTransactionsManager()
        {
            SetCredentials();
            using (var memoryStream = new MemoryStream())
            {
                RedirectConsoleOutput(memoryStream);

                _app.Run(new string[0]);

                var consoleOutput = ReadConsoleOutput(memoryStream);
                _transactions.ForEach(transaction =>
                {
                    Assert.IsTrue(consoleOutput.Contains(transaction.RunDate.Value.ToString("d")));
                    Assert.IsTrue(consoleOutput.Contains(transaction.Quantity.Value.ToString("F")));
                    Assert.IsTrue(consoleOutput.Contains(transaction.Symbol));
                    Assert.IsTrue(consoleOutput.Contains(transaction.Price.Value.ToString("C")));
                });
            }

            _transactionManagerMock.Verify(manager => manager.GetTransactionHistory(_startDate, _endDate));
        }

        [Test]
        public void ShouldSetConfigFromCliArgsWithoutPersisting()
        {
            var args = new[] {"-u", _username, "-p", _password};

            _app.Run(args);

            Assert.AreEqual(_username, _fidelityConfiguration.Username);
            Assert.AreEqual(_password, _fidelityConfiguration.Password);
            AssertUnchangedConfig();
        }

        [Test]
        public void ShouldSetConfigFromCliArgsAndPersist()
        {
            var args = new[] {"-u", _username, "-p", _password, "-s"};

            _app.Run(args);

            var fidelityConfiguration = FidelityConfigurationProviderTests.ReadConfiguration();
            Assert.AreEqual(_username, fidelityConfiguration.Username);
            Assert.AreEqual(_password, fidelityConfiguration.Password);
        }

        [Test]
        public void ShouldDisplayHelpFromCliArgsAndNotPersist()
        {
            using (var memoryStream = new MemoryStream())
            {
                RedirectConsoleOutput(memoryStream);

                _app.Run(new[] {"-u", _username, "-p", _password, "-s", "-h"});

                var consoleOutput = ReadConsoleOutput(memoryStream);
                Assert.IsTrue(consoleOutput.Contains("-h"),
                    $"Actual console output follows:{Environment.NewLine}{consoleOutput}");
                AssertUnchangedConfig();
            }
        }

        [Test]
        public void ShouldPromptForCredentials()
        {
            using (var inStream = new MemoryStream())
            {
                var inWriter = new StreamWriter(inStream) {AutoFlush = true};
                RedirectConsoleInput(inStream);
                inWriter.WriteLine(_username);
                inWriter.WriteLine(_password);
                var endlineLength = Environment.NewLine.Length;
                inStream.Position -= _username.Length + endlineLength + _password.Length + endlineLength;

                _app.Run(new string[] { });

                Assert.AreEqual(_username, _fidelityConfiguration.Username);
                Assert.AreEqual(_password, _fidelityConfiguration.Password);
            }
        }

        [Test]
        public void ShouldCascadeDisposeToManagers()
        {
            _app.Dispose();

            _positionsManagerMock.Verify(manager => manager.Dispose());
            _transactionManagerMock.Verify(manager => manager.Dispose());
        }

        [Test]
        public void ShouldHandleMultipleDisposals()
        {
            _app.Dispose();
            _app.Dispose();
        }

        private void SetCredentials()
        {
            _fidelityConfiguration.Username = _username;
            _fidelityConfiguration.Password = _password;
        }

        private static void AssertUnchangedConfig()
        {
            var fidelityConfiguration = FidelityConfigurationProviderTests.ReadConfiguration();
            Assert.IsEmpty(fidelityConfiguration.Username);
            Assert.IsEmpty(fidelityConfiguration.Password);
        }

        private static void RedirectConsoleInput(Stream memoryStream)
        {
            var streamReader = new StreamReader(memoryStream);
            Console.SetIn(streamReader);
        }

        private static void RedirectConsoleOutput(Stream memoryStream)
        {
            var streamWriter = new StreamWriter(memoryStream) {AutoFlush = true};
            Console.SetOut(streamWriter);
        }

        private static string ReadConsoleOutput(Stream memoryStream)
        {
            memoryStream.Position = 0;
            return new StreamReader(memoryStream).ReadToEnd();
        }
    }
}
