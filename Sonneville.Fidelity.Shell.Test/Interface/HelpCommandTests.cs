using System.IO;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class HelpCommandTests
    {
        private HelpCommand _command;

        [SetUp]
        public void Setup()
        {
            _command = new HelpCommand();
        }

        [Test]
        public void HasCorrectTitle()
        {
            Assert.AreEqual("help", _command.CommandName);
        }

        [Test]
        public void ShouldPrintHelp()
        {
            var outputWriterMock = new Mock<TextWriter>();
            var shouldExit = _command.Invoke(null, outputWriterMock.Object, null);
            
            Assert.IsFalse(shouldExit);
            outputWriterMock.Verify(outputWriter => outputWriter.WriteLine(It.IsAny<string>()));
        }
    }
}
