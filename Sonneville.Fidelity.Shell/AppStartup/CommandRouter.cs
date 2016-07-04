using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public interface ICommandRouter : IDisposable
    {
        void Run(IReadOnlyList<string> args);
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

        public void Run(IReadOnlyList<string> args)
        {
            if (args.Any())
            {
                RunCommand(args);
            }
            else
            {
                var exit = false;
                while (!_disposed && !exit)
                {
                    var readLine = _input.ReadLine();

                    if (readLine != null)
                    {
                        exit = RunCommand(readLine.Split(' '));
                    }
                }
                _output.WriteLine("Exiting...");
            }
        }

        private bool RunCommand(IReadOnlyList<string> tokens)
        {
            var maybeCommand = GetCommand(tokens[0]);
            maybeCommand.Invoke(_input, _output, tokens);
            return maybeCommand.ExitAfter;
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