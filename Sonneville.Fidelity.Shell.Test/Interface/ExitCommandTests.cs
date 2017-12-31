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
            Assert.IsTrue(_command.ExitAfter);
        }
    }
}
