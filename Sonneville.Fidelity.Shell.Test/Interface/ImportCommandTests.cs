using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Sonneville.Fidelity.Shell.Interface;
using Sonneville.Fidelity.WebDriver.Transactions.CSV;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Persistence;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class ImportCommandTests
    {
        private StreamReader _inputReader;
        private StreamWriter _inputWriter;
        private StreamReader _outputReader;
        private StreamWriter _outputWriter;
        private string _filePath;
        private Mock<ITransactionsMapper> _transactionsMapperMock;
        private ImportCommand _importCommand;
        private string _fileContents;
        private List<ITransaction> _fidelityTransactions;
        private Mock<ITransactionRepository> _transactionRepository;

        [SetUp]
        public void Setup()
        {
            _filePath = Path.GetTempFileName();

            _fileContents = "file contents";
            File.WriteAllText(_filePath, _fileContents);

            var inputStream = new MemoryStream();
            _inputReader = new StreamReader(inputStream);
            _inputWriter = new StreamWriter(inputStream) {AutoFlush = true};

            var outputStream = new MemoryStream();
            _outputReader = new StreamReader(outputStream);
            _outputWriter = new StreamWriter(outputStream) {AutoFlush = true};

            _fidelityTransactions = new List<ITransaction>
            {
                new Transaction()
            };
            
            _transactionsMapperMock = new Mock<ITransactionsMapper>();
            _transactionsMapperMock.Setup(transactionMapper => transactionMapper.ParseCsv(_fileContents))
                .Returns(_fidelityTransactions);

            _transactionRepository = new Mock<ITransactionRepository>();

            _importCommand = new ImportCommand(_transactionsMapperMock.Object, _transactionRepository.Object);
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_filePath)) File.Delete(_filePath);
        }

        [Test]
        public void ShouldBeNamedImport()
        {
            Assert.AreEqual("import", _importCommand.CommandName);
        }

        [Test]
        public void ShouldPassFileContentsToTransactionsMapper()
        {
            var fullInput = $"import {_filePath}".Split(' ');
            var shouldExit = InvokeWithConsole(fullInput);

            Assert.IsFalse(shouldExit);
            _transactionRepository.Verify(repository =>
                repository.Save(
                    It.Is<IEnumerable<ITransaction>>(transactions => AssertTransactionsMatch(transactions))));
        }

        [Test]
        public void ShouldThrowIfFileDoesNotExist()
        {
            File.Delete(_filePath);

            var fullInput = $"import {_filePath}".Split(' ');
            Assert.Throws<FileNotFoundException>(() => InvokeWithConsole(fullInput));
        }

        [Test]
        public void ShouldPrintHelpIfMissingFileNameArgument()
        {
            var fullInput = "import".Split(' ');
            var shouldExit = InvokeWithConsole(fullInput);

            Assert.IsFalse(shouldExit);
            AssertOutputContains("Usage:");
        }

        private bool InvokeWithConsole(IReadOnlyList<string> fullInput)
        {
            return _importCommand.Invoke(_inputReader, _outputWriter, fullInput);
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

        private bool AssertTransactionsMatch(IEnumerable<ITransaction> transactions)
        {
            CollectionAssert.AreEquivalent(_fidelityTransactions, transactions);
            return true;
        }
    }
}