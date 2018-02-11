using System;
using System.Collections.Generic;
using System.IO;

namespace Sonneville.Fidelity.Shell.Interface
{
    public interface ICommand : IDisposable
    {
        /// <summary>
        /// The name of the command. Users will invoke the command by typing the name of the command.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Invoke the command.
        /// </summary>
        /// <param name="inputReader">A <see cref="TextReader"/> used to accept input from the user. Typically <see cref="System.In"/>.</param>
        /// <param name="outputWriter">A <see cref="TextWriter"/> used to provide output to the user. Typically <see cref="System.Out"/>.</param>
        /// <param name="fullInput">The full command entered by the user. The first value will always be the name of the command.</param>
        /// <returns>A bool indicating whether or not the system should exit upon completion of the invocation.</returns>
        bool Invoke(TextReader inputReader, TextWriter outputWriter, IReadOnlyList<string> fullInput);
    }
}