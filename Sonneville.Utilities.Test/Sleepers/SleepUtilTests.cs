using System.Diagnostics;
using NUnit.Framework;
using Sonneville.Utilities.Sleepers;

namespace Sonneville.Utilities.Test.Sleepers
{
    [TestFixture]
    public class SleepUtilTests
    {
        [Test]
        [TestCase(5)]
        [TestCase(1000)]
        public void ShouldSleep(int milliseconds)
        {
            var stopwatch = Stopwatch.StartNew();

            new SleepUtil().Sleep(milliseconds);

            Assert.GreaterOrEqual(stopwatch.ElapsedMilliseconds, milliseconds);
        }
    }
}