using System;
using System.Collections.Generic;
using Sonneville.FidelityWebDriver.Configuration;

namespace Sonneville.Investing.PortfolioManager
{
    public interface IApp : IDisposable
    {
        void Run(IEnumerable<string> args);
    }

    public class App : IApp
    {
        private readonly FidelityConfiguration _fidelityConfiguration;
        private readonly IAccountRebalancer _accountRebalancer;
        private readonly ICommandLineOptionsParser _commandLineOptionsParser;

        public App(FidelityConfiguration fidelityConfiguration, ICommandLineOptionsParser commandLineOptionsParser,
            IAccountRebalancer accountRebalancer)
        {
            _fidelityConfiguration = fidelityConfiguration;
            _accountRebalancer = accountRebalancer;
            _commandLineOptionsParser = commandLineOptionsParser;
        }

        public void Run(IEnumerable<string> args)
        {
            if (!_commandLineOptionsParser.ShouldExecute(args, _fidelityConfiguration, Console.Out)) return;

            _accountRebalancer.RebalanceAccounts();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _accountRebalancer?.Dispose();
            }
        }
    }
}