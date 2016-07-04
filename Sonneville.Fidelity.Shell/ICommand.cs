﻿using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell
{
    public interface ICommand
    {
        string CommandName { get; }

        bool ExitAfter { get; }

        void Invoke(TextReader inputReader, TextWriter outputWriter, IEnumerable<string> fullInput);
    }
}