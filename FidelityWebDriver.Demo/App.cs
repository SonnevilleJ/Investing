using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NDesk.Options;
using Nini.Config;
using Sonneville.FidelityWebDriver.Configuration;
using Sonneville.FidelityWebDriver.Data;
using Sonneville.FidelityWebDriver.Demo.Ninject;
using Sonneville.FidelityWebDriver.Positions;
using Sonneville.FidelityWebDriver.Transactions;

namespace Sonneville.FidelityWebDriver.Demo
{
    public interface IApp : IDisposable
    {
        void Run(IEnumerable<string> args);
    }

    public class App : IApp
    {
        private IPositionsManager _positionsManager;

        private ITransactionManager _transactionManager;

        private readonly TransactionTranslator _transactionTranslator;

        private readonly ILog _log;
        private readonly FidelityConfiguration _fidelityConfiguration;
        private readonly OptionSet _optionSet;
        private bool _shouldPersistOptions;
        private bool _shouldShowHelp;

        public App(ILog log, IPositionsManager positionsManager, ITransactionManager transactionManager, FidelityConfiguration fidelityConfiguration, TransactionTranslator transactionTranslator)
        {
            _log = log;
            _fidelityConfiguration = fidelityConfiguration;
            _positionsManager = positionsManager;
            _transactionManager = transactionManager;
            _transactionTranslator = transactionTranslator;
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

        public void Run(IEnumerable<string> args)
        {
            _optionSet.Parse(args);
            if (_shouldShowHelp)
            {
                _optionSet.WriteOptionDescriptions(Console.Out);
                return;
            }
            if (_shouldPersistOptions)
            {
                File.Create(FidelityConfigurationProvider.ConfigLocation).Dispose();
                var iniConfigSource = new IniConfigSource(FidelityConfigurationProvider.ConfigLocation);
                iniConfigSource.AddConfig("Fidelity");
                var config = iniConfigSource.Configs["Fidelity"];
                config.Set("Username", _fidelityConfiguration.Username);
                config.Set("Password", _fidelityConfiguration.Password);
                iniConfigSource.Save();
            }
            if (string.IsNullOrEmpty(_fidelityConfiguration.Username))
            {
                _log.Info("No username configured; requesting credentials from user.");
                Console.Write("Please enter a username for Fidelity.com: ");
                _fidelityConfiguration.Username = Console.ReadLine();
                Console.Write("Please enter a password for Fidelity.com: ");
                _fidelityConfiguration.Password = Console.ReadLine();
            }

            LogToScreen("Reading account summaries.....");
            PrintAccountSummaries(_positionsManager.GetAccountSummaries().ToList());
            PrintSeparator();
            LogToScreen("Reading account details.......");
            PrintAccountDetails(_positionsManager.GetAccountDetails().ToList());
            PrintSeparator();
            LogToScreen("Reading recent transactions...");
            PrintRecentTransactions(_transactionManager.GetTransactionHistory(DateTime.Today.AddDays(-30), DateTime.Today).ToList());
            PrintSeparator();
        }

        private void PrintSeparator()
        {
            LogToScreen();
            LogToScreen("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            LogToScreen();
        }

        private void PrintAccountSummaries(IReadOnlyCollection<IAccountSummary> accountSummaries)
        {
            LogToScreen($"Found {accountSummaries.Count} accounts!");
            foreach (var account in accountSummaries)
            {
                LogToScreen($"Account Name: {account.Name}");
                LogToScreen($"Account Number: {account.AccountNumber}");
                LogToScreen($"Account Type: {account.AccountType}");
                LogToScreen($"Account Value: {account.MostRecentValue:C}");
                LogToScreen();
            }
        }

        private void PrintAccountDetails(IReadOnlyCollection<IAccountDetails> accountDetails)
        {
            LogToScreen($"Found {accountDetails.Count} accounts!");
            foreach (var accountDetail in accountDetails)
            {
                LogToScreen($"Account Name: {accountDetail.Name}");
                LogToScreen($"Account Number: {accountDetail.AccountNumber}");
                LogToScreen($"Account Type: {accountDetail.AccountType}");
                LogToScreen($"Found {accountDetail.Positions.Count()} positions in this account!");
                foreach (var position in accountDetail.Positions)
                {
                    LogToScreen($"Ticker: {position.Ticker}");
                    LogToScreen($"Shares: {position.Quantity:N}");
                    LogToScreen($"Current value: {position.CurrentValue:C}");
                    LogToScreen($"Cost basis: {position.CostBasisPerShare:C}");
                    LogToScreen();
                }
            }
        }

        private void PrintRecentTransactions(IReadOnlyCollection<IFidelityTransaction> transactions)
        {
            LogToScreen($"Found {transactions.Count} recent transactions!");
            foreach (var transaction in transactions)
            {
                LogToScreen($"On {transaction.RunDate:d} {transaction.Quantity:F} shares of {transaction.Symbol} were {_transactionTranslator.Translate(transaction.Type)} at {transaction.Price:C} per share");
            }
            LogToScreen();
        }

        private void LogToScreen(string message = null)
        {
            _log.Info(message ?? string.Empty);
            Console.WriteLine(message ?? string.Empty);
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
                _positionsManager = null;

                _transactionManager?.Dispose();
                _transactionManager = null;

                _log.Debug("App exiting...");
            }
        }
    }
}
