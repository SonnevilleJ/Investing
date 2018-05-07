using System.Threading;

namespace Sonneville.Utilities.Sleepers
{
    public interface ISleepUtil
    {
        void Sleep(int milliseconds);
    }

    public class SleepUtil : ISleepUtil
    {
        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}
