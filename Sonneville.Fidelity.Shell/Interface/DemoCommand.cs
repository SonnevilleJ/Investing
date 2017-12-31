using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NDesk.Options;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.FidelityWebDriver.Transactions;
using Sonneville.Utilities.Configuration;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class DemoCommand : ICommand
    {
        private readonly ILog _log;
        private readonly INiniConfigStore _configStore;
        private readonly IPositionsManager _positionsManager;
        private readonly ITransactionManager _transactionManager;
        private readonly TransactionTranslator _transactionTranslator;
        private readonly FidelityConfiguration _fidelityConfiguration;
        private readonly OptionSet _optionSet;
        private bool _shouldPersistOptions;
        private bool _shouldShowHelp;

        public DemoCommand(
            ILog log,
            INiniConfigStore configStore,
            IPositionsManager positionsManager,
            ITransactionManager transactionManager,
            TransactionTranslator transactionTranslator
        )
        {
            _log = log;
            _configStore = configStore;
            _positionsManager = positionsManager;
            _transactionManager = transactionManager;
            _transactionTranslator = transactionTranslator;
            
            _fidelityConfiguration = _configStore.Read<FidelityConfiguration>();
            _optionSet = new OptionSet
            {
                {
                    "u|username=", "the username to use when logging into Fidelity.",
                    username => { _fidelityConfiguration.Username = username; }
                },
                {
                    "p|password=", "the password to use when logging into Fidelity.",
                    password => { _fidelityConfiguration.Password = password; }
                },
                {
                    "s|save", "indicates options should be persisted to demo.ini file.",
                    save => { _shouldPersistOptions = true; }
                },
                {
                    "h|help", "shows this message and exits.",
                    help => { _shouldShowHelp = true; }
                },
            };

            _log.Info("App initialized");
        }

        public string CommandName { get; } = "demo";

        public bool ExitAfter { get; } = false;

        public void Invoke(TextReader inputReader, TextWriter outputWriter, IEnumerable<string> fullInput)
        {
            _optionSet.Parse(fullInput);
            if (_shouldShowHelp)
            {
                _optionSet.WriteOptionDescriptions(outputWriter);
                return;
            }

            if (_shouldPersistOptions)
            {
                _configStore.Save(_fidelityConfiguration);
            }

            if (string.IsNullOrEmpty(_fidelityConfiguration.Username))
            {
                _log.Info("No username configured; requesting credentials from user.");
                outputWriter.Write("Please enter a username for Fidelity.com: ");
                _fidelityConfiguration.Username = inputReader.ReadLine();
                outputWriter.Write("Please enter a password for Fidelity.com: ");
                _fidelityConfiguration.Password = inputReader.ReadLine();
            }

            LogToScreen(outputWriter, "Reading account summaries.....");
            PrintAccountSummaries(_positionsManager.GetAccountSummaries().ToList(), outputWriter);
            PrintSeparator(outputWriter);
            LogToScreen(outputWriter, "Reading account details.......");
            PrintAccountDetails(_positionsManager.GetAccountDetails().ToList(), outputWriter);
            PrintSeparator(outputWriter);
            LogToScreen(outputWriter, "Reading recent transactions...");
            PrintRecentTransactions(_transactionManager.GetTransactionHistory(DateTime.Today.AddDays(-30), DateTime.Today).ToList(), outputWriter);
            PrintSeparator(outputWriter);
        }

        private void PrintSeparator(TextWriter outputWriter)
        {
            LogToScreen(outputWriter);
            LogToScreen(outputWriter, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            LogToScreen(outputWriter);
        }

        private void PrintAccountSummaries(IReadOnlyCollection<IAccountSummary> accountSummaries, TextWriter outputWriter)
        {
            LogToScreen(outputWriter, $"Found {accountSummaries.Count} accounts!");
            foreach (var account in accountSummaries)
            {
                LogToScreen(outputWriter, $"Account Name: {account.Name}");
                LogToScreen(outputWriter, $"Account Number: {account.AccountNumber}");
                LogToScreen(outputWriter, $"Account Type: {account.AccountType}");
                LogToScreen(outputWriter, $"Account Value: {account.MostRecentValue:C}");
                LogToScreen(outputWriter);
            }
        }

        private void PrintAccountDetails(IReadOnlyCollection<IAccountDetails> accountDetails, TextWriter outputWriter)
        {
            LogToScreen(outputWriter, $"Found {accountDetails.Count} accounts!");
            foreach (var accountDetail in accountDetails)
            {
                LogToScreen(outputWriter, $"Account Name: {accountDetail.Name}");
                LogToScreen(outputWriter, $"Account Number: {accountDetail.AccountNumber}");
                LogToScreen(outputWriter, $"Account Type: {accountDetail.AccountType}");
                LogToScreen(outputWriter, $"Found {accountDetail.Positions.Count()} positions in this account!");
                foreach (var position in accountDetail.Positions)
                {
                    LogToScreen(outputWriter, $"Ticker: {position.Ticker}");
                    LogToScreen(outputWriter, $"Shares: {position.Quantity:N}");
                    LogToScreen(outputWriter, $"Current value: {position.CurrentValue:C}");
                    LogToScreen(outputWriter, $"Cost basis: {position.CostBasisPerShare:C}");
                    LogToScreen(outputWriter);
                }
            }
        }

        private void PrintRecentTransactions(IReadOnlyCollection<IFidelityTransaction> transactions, TextWriter outputWriter)
        {
            LogToScreen(outputWriter, $"Found {transactions.Count} recent transactions!");
            foreach (var transaction in transactions)
            {
                LogToScreen(outputWriter, $"On {transaction.RunDate:d} {transaction.Quantity:F} shares of {transaction.Symbol} were {_transactionTranslator.Translate(transaction.Type)} at {transaction.Price:C} per share");
            }

            LogToScreen(outputWriter);
        }

        private void LogToScreen(TextWriter outputWriter, string message = null)
        {
            _log.Info(message ?? string.Empty);
            outputWriter.WriteLine(message ?? string.Empty);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _positionsManager?.Dispose();
                _transactionManager?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
