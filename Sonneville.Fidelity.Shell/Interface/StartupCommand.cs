using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class StartupCommand : ICommand
    {
        public string CommandName { get; } = "startup";

        public bool Invoke(TextReader inputReader, TextWriter outputWriter, IReadOnlyList<string> fullInput)
        {
            outputWriter.WriteLine("Fidelity Shell Processor, awaiting instructions. Type `help` for instructions.");
            return false;
        }

        public void Dispose()
        {
        }
    }
}