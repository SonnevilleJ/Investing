using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class ExitCommandTests : BaseCommandTests<ExitCommand>
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            
            Command = new ExitCommand();
        }

        [Test]
        public override void HasCorrectTitle()
        {
            Assert.AreEqual("exit", Command.CommandName);
        }

        [Test]
        public override void ShouldDisposeOfDependencies()
        {
            Command.Dispose();
        }

        [Test]
        public void ShouldExitAfter()
        {
            var shouldExit = InvokeCommand();
            
            Assert.IsTrue(shouldExit);
        }
    }
}
