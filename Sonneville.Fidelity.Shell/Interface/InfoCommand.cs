﻿using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class InfoCommand : ICommand
    {
        public string CommandName { get; } = "info";

        public bool Invoke(TextReader inputReader, TextWriter outputWriter, IEnumerable<string> fullInput)
        {
            outputWriter.WriteLine("Fidelity Shell Processor, awaiting instructions. Type `help` for instructions.");
            return false;
        }

        public void Dispose()
        {
        }
    }
}