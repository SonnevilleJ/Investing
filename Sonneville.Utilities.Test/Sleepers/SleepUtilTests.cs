using System.Diagnostics;
using System.IO;
using Moq;
using NUnit.Framework;
using Sonneville.Utilities.Sleepers;

namespace Sonneville.Utilities.Test.Sleepers
{
    [TestFixture]
    public class SleepUtilTests
    {
        private ISleepUtil _sleepUtil;
        private Mock<IUnixSignalWaiter> _signalWaiterMock;
        private Mock<TextReader> _textReaderMock;

        [SetUp]
        public void Setup()
        {
            _signalWaiterMock = new Mock<IUnixSignalWaiter>();

            _textReaderMock = new Mock<TextReader>();

            _sleepUtil = new SleepUtil(_signalWaiterMock.Object, _textReaderMock.Object);
        }

        [Test]
        [TestCase(5)]
        [TestCase(1000)]
        public void ShouldSleep(int milliseconds)
        {
            var stopwatch = Stopwatch.StartNew();

            new SleepUtil().Sleep(milliseconds);

            Assert.GreaterOrEqual(stopwatch.ElapsedMilliseconds, milliseconds);
        }

        [Test]
        public void ShouldUseWaiterIfPossible()
        {
            _signalWaiterMock.Setup(waiter => waiter.CanWaitExitSignal()).Returns(true);

            _sleepUtil.WaitForExitSignal();

            _signalWaiterMock.Verify(waiter => waiter.WaitExitSignal());
        }

        [Test]
        public void ShouldReadConsoleLineIfUnableToUseWaiter()
        {
            _signalWaiterMock.Setup(waiter => waiter.CanWaitExitSignal()).Returns(false);

            _sleepUtil.WaitForExitSignal();

            _textReaderMock.Verify(reader => reader.ReadLine());
            _signalWaiterMock.Verify(waiter => waiter.WaitExitSignal(), Times.Never);
        }
    }
}