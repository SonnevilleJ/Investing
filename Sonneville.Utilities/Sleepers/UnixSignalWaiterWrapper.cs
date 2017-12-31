using UnixSignalWaiter;

namespace Sonneville.Utilities.Sleepers
{
    public interface IUnixSignalWaiter
    {
        bool CanWaitExitSignal();

        void WaitExitSignal();
    }

    public class UnixSignalWaiterWrapper : IUnixSignalWaiter
    {
        private readonly SignalWaiter _unixSignalWaiter;

        public UnixSignalWaiterWrapper()
        {
            _unixSignalWaiter = SignalWaiter.Instance;
        }

        public bool CanWaitExitSignal()
        {
            return _unixSignalWaiter.CanWaitExitSignal();
        }

        public void WaitExitSignal()
        {
            _unixSignalWaiter.WaitExitSignal();
        }
    }
}