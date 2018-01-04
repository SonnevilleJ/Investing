using System;
using System.Threading;
using NUnit.Framework;

namespace Sonneville.Utilities.Test
{
    [TestFixture]
    public class FreezableClockTests
    {
        [Test]
        public void ShouldReturnActualTimeBeforeFrozen()
        {
            var clock = new FreezableClock();

            Assert.AreEqual(DateTime.Today, clock.Now.Date);
            Thread.Sleep(100);
            Assert.AreEqual(DateTime.Today, clock.Now.Date);
        }

        [Test]
        public void ShouldReturnSameTimeIfFrozen()
        {
            var clock = new FreezableClock();
            var dateTime = new DateTime(2018, 1, 4, 9, 49, 13);
            clock.Freeze(dateTime);

            Assert.AreEqual(dateTime, clock.Now);
            Thread.Sleep(100);
            Assert.AreEqual(dateTime, clock.Now);
        }
        
        [Test]
        public void ShouldReturnActualTimeAfterFrozen()
        {
            var clock = new FreezableClock();
            var time = new DateTime(2018, 1, 4, 9, 49, 13);
            clock.Freeze(time);
            clock.Unfreeze();

            Assert.AreEqual(DateTime.Today, clock.Now.Date);
            Thread.Sleep(100);
            Assert.AreEqual(DateTime.Today, clock.Now.Date);
        }
    }
}
