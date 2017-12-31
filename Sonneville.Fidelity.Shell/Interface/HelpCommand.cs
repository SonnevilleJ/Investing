using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class HelpCommand : ICommand
    {
        public string CommandName { get; } = "help";

        public bool ExitAfter { get; } = false;

        public void Invoke(TextReader inputReader, TextWriter outputWriter, IEnumerable<string> fullInput)
        {
            outputWriter.WriteLine("Type `exit` to exit.");
        }

        public void Dispose()
        {
        }
    }
}