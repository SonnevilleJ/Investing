using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using Sonneville.Fidelity.Shell.Interface;
using Sonneville.Investing.Domain;
using Sonneville.Investing.Fidelity.CSV;
using Sonneville.Investing.Persistence;

namespace Sonneville.Fidelity.Shell.Test.Interface
{
    [TestFixture]
    public class ImportCommandTests : BaseCommandTests<ImportCommand>
    {
        private string _filePath;
        private Mock<ITransactionsMapper> _transactionsMapperMock;
        private string _fileContents;
        private List<ITransaction> _fidelityTransactions;
        private Mock<ITransactionRepository> _transactionRepository;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _filePath = Path.GetTempFileName();

            _fileContents = "file contents";
            File.WriteAllText(_filePath, _fileContents);

            _fidelityTransactions = new List<ITransaction>
            {
                new Transaction()
            };

            _transactionsMapperMock = new Mock<ITransactionsMapper>();
            _transactionsMapperMock.Setup(transactionMapper => transactionMapper.ParseCsv(_fileContents))
                .Returns(_fidelityTransactions);

            _transactionRepository = new Mock<ITransactionRepository>();

            Command = new ImportCommand(_transactionsMapperMock.Object, _transactionRepository.Object);
        }

        [TearDown]
        public override void Teardown()
        {
            if (File.Exists(_filePath)) File.Delete(_filePath);

            base.Teardown();
        }

        public override void HasCorrectTitle()
        {
            Assert.AreEqual("import", Command.CommandName);
        }

        [Test]
        public override void ShouldDisposeOfDependencies()
        {
            Command.Dispose();
        }

        [Test]
        public void ShouldPassFileContentsToTransactionsMapper()
        {
            var shouldExit = InvokeCommand($"import {_filePath}".Split(' '));

            Assert.IsFalse(shouldExit);
            _transactionRepository.Verify(repository => repository.Save(
                It.Is<IEnumerable<ITransaction>>(
                    transactions => AssertEquals(_fidelityTransactions, transactions)
                )
            ));
        }

        [Test]
        public void ShouldThrowIfFileDoesNotExist()
        {
            File.Delete(_filePath);

            Assert.Throws<FileNotFoundException>(() => InvokeCommand($"import {_filePath}".Split(' ')));
        }

        [Test]
        public void ShouldPrintHelpIfMissingFileNameArgument()
        {
            var shouldExit = InvokeCommand("import".Split(' '));

            Assert.IsFalse(shouldExit);
            AssertOutputContains("Usage:");
        }
    }
}
