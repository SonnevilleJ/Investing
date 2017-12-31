using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class ExitCommand : ICommand
    {
        public string CommandName { get; } = "exit";

        public bool ExitAfter { get; } = true;

        public void Invoke(TextReader inputReader, TextWriter outputWriter, IEnumerable<string> fullInput)
        {
            outputWriter.WriteLine("Exiting...");
        }

        public void Dispose()
        {
        }
    }
}
