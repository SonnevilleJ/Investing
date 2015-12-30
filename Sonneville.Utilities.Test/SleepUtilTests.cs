using System.Diagnostics;
using NUnit.Framework;

namespace Sonneville.Utilities.Test
{
    [TestFixture]
    public class SleepUtilTests
    {
        private ISleepUtil _sleepUtil;

        [SetUp]
        public void Setup()
        {
            _sleepUtil = new SleepUtil();
        }

        [Test]
        [TestCase(5)]
        [TestCase(1000)]
        public void ShouldSleep(int milliseconds)
        {
            var stopwatch = Stopwatch.StartNew();

            _sleepUtil.Sleep(milliseconds);

            Assert.GreaterOrEqual(stopwatch.ElapsedMilliseconds, milliseconds);
        }
    }
}