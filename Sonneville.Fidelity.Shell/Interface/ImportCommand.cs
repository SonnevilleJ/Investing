using System.Collections.Generic;
using System.IO;
using Sonneville.Investing.Fidelity.CSV;
using Sonneville.Investing.Persistence;

namespace Sonneville.Fidelity.Shell.Interface
{
    public class ImportCommand : ICommand
    {
        private readonly ITransactionsMapper _transactionsMapper;
        private readonly ITransactionRepository _transactionRepository;

        public ImportCommand(ITransactionsMapper transactionsMapper, ITransactionRepository transactionRepository)
        {
            _transactionsMapper = transactionsMapper;
            _transactionRepository = transactionRepository;
        }

        public string CommandName => "import";

        public bool Invoke(TextReader inputReader, TextWriter outputWriter, IReadOnlyList<string> fullInput)
        {
            if (fullInput.Count >= 2)
            {
                var filePath = fullInput[1];
                var csvContent = File.ReadAllText(filePath);
                outputWriter.WriteLine(csvContent);
                var transactions = _transactionsMapper.ParseCsv(csvContent);
                _transactionRepository.Save(transactions);

                return false;
            }

            PrintUsage(outputWriter);
            return false;
        }

        private void PrintUsage(TextWriter outputWriter)
        {
            outputWriter.WriteLine("Usage:");
            outputWriter.WriteLine($"{CommandName} [FILE]");
            outputWriter.WriteLine("Parse transactions from a FILE.");
        }

        public void Dispose()
        {
        }
    }
}