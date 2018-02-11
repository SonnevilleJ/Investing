using System.IO;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class StartupCommandTests
    {
        private StartupCommand _command;

        [SetUp]
        public void Setup()
        {
            _command = new StartupCommand();
        }

        [Test]
        public void HasCorrectTitle()
        {
            Assert.AreEqual("startup", _command.CommandName);
        }

        [Test]
        public void ShouldGreetWhenInvoked()
        {
            var outputWriterMock = new Mock<TextWriter>();
            var shouldExit = _command.Invoke(null, outputWriterMock.Object, null);

            Assert.IsFalse(shouldExit);
            outputWriterMock.Verify(writer => writer.WriteLine(It.IsAny<string>()));
        }
    }
}
