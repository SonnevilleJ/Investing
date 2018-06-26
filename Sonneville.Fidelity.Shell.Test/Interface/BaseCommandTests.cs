using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Sonneville.Fidelity.Shell.Interface;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public abstract class BaseCommandTests<TCommand> where TCommand : ICommand
    {
        private MemoryStream _inputStream;
        private StreamReader _inputReader;
        private StreamWriter _inputWriter;
        private MemoryStream _outputStream;
        private StreamReader _outputReader;
        private StreamWriter _outputWriter;

        protected TCommand Command;

        [SetUp]
        public virtual void Setup()
        {
            _inputStream = new MemoryStream();
            _inputReader = new StreamReader(_inputStream);
            _inputWriter = new StreamWriter(_inputStream) {AutoFlush = true};

            _outputStream = new MemoryStream();
            _outputReader = new StreamReader(_outputStream);
            _outputWriter = new StreamWriter(_outputStream) {AutoFlush = true};
        }

        [TearDown]
        public virtual void Teardown()
        {
            _inputStream.Dispose();
            _inputReader.Dispose();
            _inputWriter.Dispose();

            _outputStream.Dispose();
            _outputReader.Dispose();
            _outputWriter.Dispose();

            Command?.Dispose();
        }

        [Test]
        public abstract void HasCorrectTitle();

        [Test]
        public abstract void ShouldDisposeOfDependencies();

        [Test]
        public void ShouldHandleMultipleDisposals()
        {
            Command.Dispose();
            Command.Dispose();
        }

        protected static bool AssertEquals<TObject>(TObject expected, TObject actual)
        {
            Assert.AreEqual(expected, actual);
            return true;
        }

        protected bool InvokeCommand(IReadOnlyList<string> fullInput = null)
        {
            return Command.Invoke(_inputReader, _outputWriter, fullInput ?? new string[0]);
        }

        protected void EnqueueInput(params string[] input)
        {
            var distance = 0;
            foreach (var line in input)
            {
                _inputWriter.WriteLine(line);
                distance += line.Length + Environment.NewLine.Length;
            }

            _inputStream.Position -= distance;
        }

        protected void AssertOutputContains(string value)
        {
            var outputText = ReadOutputText();
            Assert.That(
                outputText,
                new ContainsConstraint(value),
                $"Actual console output follows:{Environment.NewLine}{outputText}"
            );
        }

        private string ReadOutputText()
        {
            _outputReader.BaseStream.Position = 0;
            return _outputReader.ReadToEnd();
        }
    }
}
