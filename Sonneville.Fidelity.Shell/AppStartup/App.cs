using System;
using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell.AppStartup
{
    public interface IApp : IDisposable
    {
        void Run(IEnumerable<string> args);
    }

    public class App : IApp
    {
        private readonly IAccountRebalancer _accountRebalancer;
        private readonly TextReader _input;
        private readonly TextWriter _output;
        private bool _disposed;

        public App(IAccountRebalancer accountRebalancer, TextReader input, TextWriter output)
        {
            _accountRebalancer = accountRebalancer;
            _input = input;
            _output = output;
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

                    switch (lines[0].ToLowerInvariant())
                    {
                        case "help":
                            PrintHelp(_output);
                            break;
                        case "exit":
                            exit = true;
                            break;
                        default:
                            _output.WriteLine($"Unknown command: {lines[0]}");
                            PrintHelp(_output);
                            break;
                    }
                }
            }
            _output.WriteLine("Exiting...");
        }

        private static void PrintHelp(TextWriter textWriter)
        {
            textWriter.WriteLine("usage");
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
                _accountRebalancer?.Dispose();
            }
        }
    }
}