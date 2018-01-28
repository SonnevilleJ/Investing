using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class ExitCommand : ICommand
    {
        public string CommandName { get; } = "exit";

        public bool Invoke(TextReader inputReader, TextWriter outputWriter, IEnumerable<string> fullInput)
        {
            outputWriter.WriteLine("Exiting...");
            return true;
        }

        public void Dispose()
        {
        }
    }
}
