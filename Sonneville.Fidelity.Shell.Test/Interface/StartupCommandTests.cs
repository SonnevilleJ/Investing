using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class StartupCommandTests : BaseCommandTests<StartupCommand>
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            Command = new StartupCommand();
        }

        [Test]
        public override void HasCorrectTitle()
        {
            Assert.AreEqual("startup", Command.CommandName);
        }

        [Test]
        public override void ShouldDisposeOfDependencies()
        {
            Command.Dispose();
        }

        [Test]
        public void ShouldGreetWhenInvoked()
        {
            var shouldExit = InvokeCommand();

            Assert.IsFalse(shouldExit);
            AssertOutputContains(" ");
        }
    }
}
