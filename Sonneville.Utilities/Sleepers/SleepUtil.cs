using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Sonneville.Utilities.Sleepers
{
    public interface ISleepUtil
    {
        void Sleep(int milliseconds);

        void WaitForExitSignal();
    }

    public class SleepUtil : ISleepUtil
    {
        private readonly IUnixSignalWaiter _unixSignalWaiter;
        private readonly TextReader _consoleIn;

        public SleepUtil() : this(new UnixSignalWaiterWrapper(), Console.In)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public SleepUtil(IUnixSignalWaiter unixSignalWaiter, TextReader textReader)
        {
            _unixSignalWaiter = unixSignalWaiter;
            _consoleIn = textReader;
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public void WaitForExitSignal()
        {
            if (_unixSignalWaiter.CanWaitExitSignal())
            {
                _unixSignalWaiter.WaitExitSignal();
            }
            else
            {
                _consoleIn.ReadLine();
            }
        }
    }
}
