using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using log4net;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.WebDriver.Configuration;
using Sonneville.Investing.Fidelity.WebDriver.Positions;
using Sonneville.Investing.Fidelity.WebDriver.Transactions;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class DemoCommand : ICommand
    {
        private readonly ILog _log;
        private readonly IDataStore _dataStore;
        private readonly IPositionsManager _positionsManager;
        private readonly ITransactionManager _transactionManager;
        private readonly TransactionTranslator _transactionTranslator;
        private readonly FidelityConfiguration _fidelityConfiguration;

        public DemoCommand(
            ILog log,
            IDataStore dataStore,
            IPositionsManager positionsManager,
            ITransactionManager transactionManager,
            TransactionTranslator transactionTranslator
        )
        {
            _log = log;
            _dataStore = dataStore;
            _positionsManager = positionsManager;
            _transactionManager = transactionManager;
            _transactionTranslator = transactionTranslator;
            _fidelityConfiguration = _dataStore.Load<FidelityConfiguration>();
            _log.Info("App initialized");
        }

        public string CommandName { get; } = "demo";

        public bool Invoke(TextReader inputReader, TextWriter outputWriter, IReadOnlyList<string> fullInput)
        {
            var run = false;
            var parser = new Parser(settings => settings.HelpWriter = outputWriter);
            parser.ParseArguments<DemoOptions>(fullInput).WithParsed(options =>
            {
                if (!string.IsNullOrEmpty(options.Username))
                {
                    _fidelityConfiguration.Username = options.Username;
                }

                if (!string.IsNullOrEmpty(options.Password))
                {
                    _fidelityConfiguration.Password = options.Password;
                }

                if (options.SaveOptions)
                {
                    var message = $"Saving credentials for `{_fidelityConfiguration.Username}`.";
                    _log.Info(message);
                    _dataStore.Save(_fidelityConfiguration);
                }

                if (!string.IsNullOrEmpty(_fidelityConfiguration.Username) &&
                    !string.IsNullOrEmpty(_fidelityConfiguration.Password))
                {
                    var message =
                        $"Using cached credentials to access account for user `{_fidelityConfiguration.Username}`.";
                    _log.Info(message);
                }
                else
                {
                    _log.Info("No username configured; requesting credentials from user.");
                    outputWriter.Write("Please enter a username for Fidelity.com: ");
                    _fidelityConfiguration.Username = inputReader.ReadLine();
                    outputWriter.Write("Please enter a password for Fidelity.com: ");
                    _fidelityConfiguration.Password = inputReader.ReadLine();
                }

                run = true;
            });
            if (run)
            {
                LogToScreen(outputWriter, "Reading account summaries.....");
                PrintAccountSummaries(_positionsManager.GetAccountSummaries().ToList(), outputWriter);
                PrintSeparator(outputWriter);
                LogToScreen(outputWriter, "Reading account details.......");
                PrintAccountDetails(_positionsManager.GetAccountDetails().ToList(), outputWriter);
                PrintSeparator(outputWriter);
                LogToScreen(outputWriter, "Reading recent transactions...");

                PrintRecentTransactions(
                    _transactionManager.GetTransactionHistory(DateTime.Today.AddDays(-30), DateTime.Today).ToList()
                    , outputWriter);
                PrintSeparator(outputWriter);

                LogToScreen(outputWriter, "Demo completed successfully!");
            }

            return false;
        }

        private void PrintSeparator(TextWriter outputWriter)
        {
            LogToScreen(outputWriter);

            LogToScreen(outputWriter,
                "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            LogToScreen(outputWriter);
        }

        private void PrintAccountSummaries(IReadOnlyCollection<IAccountSummary> accountSummaries,
            TextWriter outputWriter)
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

        private void PrintRecentTransactions(IReadOnlyCollection<ITransaction> transactions,
            TextWriter outputWriter)
        {
            LogToScreen(outputWriter, $"Found {transactions.Count} recent transactions!");
            foreach (var transaction in transactions)
            {
                LogToScreen(outputWriter,
                    $"On {transaction.RunDate:d} {transaction.Quantity:F} shares of {transaction.Symbol} were {_transactionTranslator.Translate(transaction.Type)} at {transaction.Price:C} per share");
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
