using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public interface ICommandRouter : IDisposable
    {
        void Run(IEnumerable<string> args);
    }

    public class CommandRouter : ICommandRouter
    {
        private readonly TextReader _input;
        private readonly TextWriter _output;
        private readonly ICollection<ICommand> _commands;
        private bool _disposed;

        public CommandRouter(TextReader input, TextWriter output, ICollection<ICommand> commands)
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
                    var maybeCommand = GetCommand(readLine.Split(' ')[0]);
                    maybeCommand.Invoke(_input, _output, readLine);
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