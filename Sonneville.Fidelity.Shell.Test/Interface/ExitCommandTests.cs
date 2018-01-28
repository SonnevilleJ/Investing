using System.IO;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class ExitCommandTests
    {
        private ExitCommand _command;

        [SetUp]
        public void Setup()
        {
            _command = new ExitCommand();
        }

        [Test]
        public void HasCorrectTitle()
        {
            Assert.AreEqual("exit", _command.CommandName);
        }

        [Test]
        public void ShouldExitAfter()
        {
            var outputWriterMock = new Mock<TextWriter>();
            
            var shouldExit = _command.Invoke(null, outputWriterMock.Object, null);
            
            Assert.IsTrue(shouldExit);
        }
    }
}
