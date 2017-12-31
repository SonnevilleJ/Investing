﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell.Interface
{
    public interface ICommand : IDisposable
    {
        string CommandName { get; }

        bool ExitAfter { get; }

        void Invoke(TextReader inputReader, TextWriter outputWriter, IEnumerable<string> fullInput);
    }
}