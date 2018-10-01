using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sonneville.Investing.Fidelity.WebDriver.Logging;

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
        private readonly IExceptionReportGenerator _exceptionReportGenerator;
        private bool _disposed;

        public CommandRouter(
            TextReader inputReader,
            TextWriter outputWriter,
            ICommand[] commands,
            IExceptionReportGenerator exceptionReportGenerator)
        {
            _inputReader = inputReader;
            _outputWriter = outputWriter;
            _commands = commands;
            _exceptionReportGenerator = exceptionReportGenerator;
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
                var result = _exceptionReportGenerator.DocumentException(e);
                _outputWriter.WriteLine(e);
                _outputWriter.WriteLine();
                result.MatchSome(location => _outputWriter.WriteLine($"Wrote exception report to: {location}"));
                _outputWriter.WriteLine("Error occurred! Please consider submitting a bug report! ;)");
                _outputWriter.WriteLine("Press enter key to exit...");
                _inputReader.ReadLine();
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

                _exceptionReportGenerator.Dispose();

                _disposed = true;
            }
        }
    }
}