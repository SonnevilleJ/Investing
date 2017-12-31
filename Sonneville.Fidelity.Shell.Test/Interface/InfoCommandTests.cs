using System.IO;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class InfoCommandTests
    {
        private InfoCommand _command;

        [SetUp]
        public void Setup()
        {
            _command = new InfoCommand();
        }

        [Test]
        public void HasCorrectTitle()
        {
            Assert.AreEqual("info", _command.CommandName);
        }

        [Test]
        public void ShouldNotExitAfter()
        {
            Assert.IsFalse(_command.ExitAfter);
        }

        [Test]
        public void ShouldGreetWhenInvoked()
        {
            var outputMock = new Mock<TextWriter>();
            _command.Invoke(null, outputMock.Object, null);
            
            outputMock.Verify(writer => writer.WriteLine(It.IsAny<string>()));
        }
    }
}
