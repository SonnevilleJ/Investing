using System;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class HelpCommandTests : BaseCommandTests<HelpCommand>
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            Command = new HelpCommand();
        }

        [Test]
        public override void HasCorrectTitle()
        {
            Assert.AreEqual("help", Command.CommandName);
        }

        [Test]
        public override void ShouldDisposeOfDependencies()
        {
            Command.Dispose();
        }

        [Test]
        public void ShouldPrintHelp()
        {
            var shouldExit = InvokeCommand();
            
            Assert.IsFalse(shouldExit);
            AssertOutputContains(Environment.NewLine);
        }
    }
}
