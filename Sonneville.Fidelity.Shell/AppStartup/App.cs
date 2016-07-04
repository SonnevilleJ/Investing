using System;
using System.Collections.Generic;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public interface IApp : IDisposable
    {
        void Run(IEnumerable<string> args);
    }

    public class App : IApp
    {
        private readonly IAccountRebalancer _accountRebalancer;
        private readonly ICommandLineOptionsParser _commandLineOptionsParser;

        public App(ICommandLineOptionsParser commandLineOptionsParser,
            IAccountRebalancer accountRebalancer)
        {
            _accountRebalancer = accountRebalancer;
            _commandLineOptionsParser = commandLineOptionsParser;
        }

        public void Run(IEnumerable<string> args)
        {
            if (!_commandLineOptionsParser.ShouldExecute(args, Console.Out)) return;

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