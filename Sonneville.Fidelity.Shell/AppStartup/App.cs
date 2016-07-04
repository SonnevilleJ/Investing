using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public interface IApp : IDisposable
    {
        void Run(IEnumerable<string> args);
    }

    public class App : IApp
    {
        private readonly TextReader _input;
        private readonly TextWriter _output;
        private readonly ICollection<ICommand> _commands;
        private bool _disposed;

        public App(TextReader input, TextWriter output, ICollection<ICommand> commands)
        {
            _input = input;
            _output = output;
            _commands = commands;
        }

        public void Run(IEnumerable<string> args)
        {
            var exit = false;
            while (!_disposed && !exit)
            {
                var readLine = _input.ReadLine();

                if (readLine != null)
                {
                    var lines = readLine.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                    var maybeCommand = GetCommand(lines[0].ToLowerInvariant());
                    maybeCommand.Invoke(_input, _output);
                    if (maybeCommand.ExitAfter)
                        exit = true;
                }
            }
            _output.WriteLine("Exiting...");
        }

        private ICommand GetCommand(string commandName)
        {
            return _commands.SingleOrDefault(c => c.CommandName == commandName) ??
                   _commands.Single(c => c.CommandName == "help");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposed = true;
            }
        }
    }
}