using System.Threading;

namespace Sonneville.Utilities
{
    public class SleepUtil
    {
        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }
    }
}