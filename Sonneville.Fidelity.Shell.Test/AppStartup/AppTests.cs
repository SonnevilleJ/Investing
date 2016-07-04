using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.AppStartup;

namespace Sonneville.Fidelity.Shell.Test.AppStartup
{
    [TestFixture]
    public class AppTests
    {
        private App _app;
        private string[] _cliArgs;
        private StreamWriter _inputWriter;
        private StreamReader _outputReader;
        private Task _task;
        private List<ICommand> _commands;
        private StreamReader _inputReader;
        private StreamWriter _outputWriter;

        [SetUp]
        public void Setup()
        {
            _cliArgs = new string[0];

            var inputStream = new MemoryStream();
            _inputReader = new StreamReader(inputStream);
            _inputWriter = new StreamWriter(inputStream) {AutoFlush = true};

            var outputStream = new MemoryStream();
            _outputReader = new StreamReader(outputStream);
            _outputWriter = new StreamWriter(outputStream) {AutoFlush = true};

            _commands = new List<ICommand>
            {
                CreateCommand("help", false),
                CreateCommand("exit", true)
            };


            _app = new App(_inputReader, _outputWriter, _commands);
        }

        [TearDown]
        public void Teardown()
        {
            _inputWriter.Dispose();
            _outputReader.Dispose();
            _app.Dispose();
        }

        [Test]
        public void DisposeShouldNotThrow()
        {
            Assert.DoesNotThrow(() => _app.Dispose());
        }

        [Test]
        public void RunShouldWaitForExitCommand()
        {
            _task = Task.Run(() => _app.Run(_cliArgs));
            _task.Wait(1000);
            Assert.IsFalse(_task.IsCompleted);

            SendInput("exit");

            AssertCommandWasInvoked("exit");
            Assert.IsTrue(_task.IsCompleted);
        }

        [Test]
        public void HelpCommandShouldPrintHelp()
        {
            _task = Task.Run(() => _app.Run(_cliArgs));

            SendInput("help");

            AssertCommandWasInvoked("help");
        }

        [Test]
        public void UnknownCommandShouldPrintHelp()
        {
            _task = Task.Run(() => _app.Run(_cliArgs));

            SendInput("asdf");

            AssertCommandWasInvoked("help");
        }

        private ICommand CreateCommand(string commandName, bool exitAfter)
        {
            var mockCommand = new Mock<ICommand>();
            mockCommand.SetupGet(command => command.CommandName).Returns(commandName);
            mockCommand.SetupGet(command => command.ExitAfter).Returns(exitAfter);
            return mockCommand.Object;
        }

        private void SendInput(string text)
        {
            _inputWriter.BaseStream.Position = 0;
            _inputWriter.WriteLine(text);
            _inputWriter.BaseStream.Position = 0;
            _task.Wait(100);
        }

        private void AssertCommandWasInvoked(string commandName)
        {
            Mock.Get(_commands.Single(command => command.CommandName == commandName))
                .Verify(command => command.Invoke(_inputReader, _outputWriter));
        }
    }
}