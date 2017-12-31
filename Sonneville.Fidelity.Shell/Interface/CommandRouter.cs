using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonneville.Fidelity.Shell.Interface
{
    public interface ICommandRouter : IDisposable
    {
        void Run(IReadOnlyList<string> args);
    }

    public class CommandRouter : ICommandRouter
    {
        private readonly TextReader _inputReader;
        private readonly TextWriter _outputWriter;
        private readonly IReadOnlyCollection<ICommand> _commands;
        private bool _disposed;

        public CommandRouter(TextReader inputReader, TextWriter outputWriter, ICommand[] commands)
        {
            _inputReader = inputReader;
            _outputWriter = outputWriter;
            _commands = commands;
        }

        public void Run(IReadOnlyList<string> args)
        {
            if (args.Any())
            {
                RunCommand(args);
            }
            else
            {
                var exit = RunCommand(new[] {"info"});
                while (!_disposed && !exit)
                {
                    var commandString = _inputReader.ReadLine();

                    if (commandString != null)
                    {
                        exit = RunCommand(commandString.Split(' '));
                    }
                }
            }
        }

        private bool RunCommand(IReadOnlyList<string> tokens)
        {
            var command = GetCommand(tokens[0].ToLowerInvariant());
            command.Invoke(_inputReader, _outputWriter, tokens);
            return command.ExitAfter;
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
                foreach (var command in _commands)
                {
                    command.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
