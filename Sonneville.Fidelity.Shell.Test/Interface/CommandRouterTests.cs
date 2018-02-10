﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class CommandRouterTests
    {
        private string[] _cliArgs;
        private StreamReader _inputReader;
        private StreamWriter _inputWriter;
        private StreamReader _outputReader;
        private StreamWriter _outputWriter;
        private Task _task;
        private List<ICommand> _commands;
        private CommandRouter _commandRouter;

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
                CreateCommand("info", false),
                CreateCommand("exit", true)
            };

            _commandRouter = new CommandRouter(_inputReader, _outputWriter, _commands.ToArray());
        }

        [TearDown]
        public void Teardown()
        {
            _inputWriter.Dispose();
            _outputReader.Dispose();
            _commandRouter.Dispose();
        }

        [Test]
        public void ShouldDisposeCommands()
        {
            _commandRouter.Dispose();

            _commands.ForEach(command => Mock.Get(command).Verify(mock => mock.Dispose(), Times.Once()));
        }

        [Test]
        public void DisposeShouldNotThrow()
        {
            Assert.DoesNotThrow(() => _commandRouter.Dispose());
        }

        [Test]
        [TestCase("asdf", false)]
        public void ShouldInvokeHelpIfCommandNotFound(string fullInput, bool shouldExit)
        {
            _task = Task.Run(() => _commandRouter.Run(_cliArgs));

            SendInput(fullInput);
            _task.Wait(100);

            var inputArray = fullInput.Split(' ');
            AssertCommandWasInvoked("help", inputArray);
            AssertOutputContains($"Command not found: {inputArray.First()}");
            Assert.AreEqual(shouldExit, _task.IsCompleted);
        }

        [Test]
        [TestCase("help", "help", false)]
        [TestCase("help", "help 1 2 3 4", false)]
        [TestCase("info", "info", false)]
        [TestCase("exit", "exit", true)]
        public void ShouldInvokeCommandFromInputAndWait(string expectedCommand, string fullInput, bool shouldExit)
        {
            _task = Task.Run(() => _commandRouter.Run(_cliArgs));

            SendInput(fullInput);
            _task.Wait(100);

            AssertCommandWasInvoked(expectedCommand, fullInput.Split(' '), expectedCommand == "info" ? 2 : 1);
            Assert.AreEqual(shouldExit, _task.IsCompleted);
        }

        [Test]
        [TestCase("exit", "EXIT", true)]
        [TestCase("exit", "eXiT", true)]
        public void ShouldBeCaseInsensitive(string expectedCommand, string fullInput, bool shouldExit)
        {
            _task = Task.Run(() => _commandRouter.Run(_cliArgs));

            SendInput(fullInput);
            _task.Wait(100);

            AssertCommandWasInvoked(expectedCommand, fullInput.Split(' '));
            Assert.AreEqual(shouldExit, _task.IsCompleted);
        }

        [Test]
        public void ShouldInvokeInfoCommandAndWait()
        {
            _task = Task.Run(() => _commandRouter.Run(_cliArgs));
            _task.Wait(100);

            AssertCommandWasInvoked("info", new[] {"info"});
            Assert.IsFalse(_task.IsCompleted);
        }

        [Test]
        [TestCase("help", "help 1 2 3 4")]
        [TestCase("help", "asdf")]
        [TestCase("exit", "exit")]
        public void ShouldInvokeCommandFromCliArgsThenExit(string expectedCommand, string fullInput)
        {
            var cliArgs = fullInput.Split(' ');

            _task = Task.Run(() => _commandRouter.Run(cliArgs));
            _task.Wait(100);

            AssertCommandWasInvoked(expectedCommand, cliArgs);
            Assert.IsTrue(_task.IsCompleted);
        }

        private ICommand CreateCommand(string commandName, bool exitAfter)
        {
            var mockCommand = new Mock<ICommand>();
            mockCommand.SetupGet(command => command.CommandName).Returns(commandName);
            mockCommand.Setup(command => command.Invoke(_inputReader, _outputWriter, It.IsAny<IEnumerable<string>>())).Returns(exitAfter);
            return mockCommand.Object;
        }

        private void SendInput(string text)
        {
            _inputWriter.BaseStream.Position = 0;
            _inputWriter.WriteLine(text);
            _inputWriter.BaseStream.Position = 0;
            _task.Wait(100);
        }

        private void AssertCommandWasInvoked(string commandName, IReadOnlyList<string> fullInput, int times = 1)
        {
            Mock.Get(_commands.Single(command => command.CommandName == commandName))
                .Verify(command => command.Invoke(_inputReader, _outputWriter, fullInput), Times.Exactly(times));
        }

        private void AssertOutputContains(string value)
        {
            Assert.That(ReadOutputText(), new ContainsConstraint(value));
        }

        private string ReadOutputText()
        {
            _outputReader.BaseStream.Position = 0;
            return _outputReader.ReadToEnd();
        }
    }
}
