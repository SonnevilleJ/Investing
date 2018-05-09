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
                var exit = RunCommand(new[] {"startup"});
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
            try
            {
                var command = GetCommand(tokens[0].ToLowerInvariant());
                return command.Invoke(_inputReader, _outputWriter, tokens.ToArray());
            }
            catch (Exception e)
            {
                _outputWriter.WriteLine(e);
                _outputWriter.WriteLine("Error occurred! Please consider submitting a bug report! ;)");
                _outputWriter.WriteLine("Press any key to exit...");
                _inputReader.Read();
                return true;
            }
        }

        private ICommand GetCommand(string commandName)
        {
            var command = _commands.SingleOrDefault(c => c.CommandName == commandName);
            if (command != null) return command;
            _outputWriter.WriteLine("Command not found: " + commandName);
            return _commands.Single(c => c.CommandName == "help");
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