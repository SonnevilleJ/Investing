using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class HelpCommand : ICommand
    {
        public string CommandName { get; } = "help";

        public bool Invoke(TextReader inputReader, TextWriter outputWriter, IEnumerable<string> fullInput)
        {
            outputWriter.WriteLine("Type `exit` to exit.");
            return false;
        }

        public void Dispose()
        {
        }
    }
}