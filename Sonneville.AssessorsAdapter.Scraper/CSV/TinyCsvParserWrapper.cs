using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace Sonneville.AssessorsAdapter.Scraper.CSV
{
    public class TinyCsvParserWrapper<TEntity> : ICsvParser<TEntity> where TEntity : class, new()
    {
        private readonly CsvParser<TEntity> _csvParser;

        private TinyCsvParserWrapper(CsvParser<TEntity> csvParser)
        {
            _csvParser = csvParser;
        }

        public ParallelQuery<CsvMappingResult<TEntity>> ReadFromFile(
            string fileName,
            Encoding encoding)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            var csvData = File.ReadLines(fileName, encoding)
                .Select((line, index) => new Row(index, line));
            return Parse(csvData);
        }

        public ParallelQuery<CsvMappingResult<TEntity>> ReadFromString(
            CsvReaderOptions csvReaderOptions,
            string csvData)
        {
            var csvData1 =
                csvData.Split(csvReaderOptions.NewLine, StringSplitOptions.None)
                    .Select((line, index) => new Row(index, line));
            return Parse(csvData1);
        }

        public ParallelQuery<CsvMappingResult<TEntity>> Parse(IEnumerable<Row> csvData)
        {
            return _csvParser.Parse(csvData);
        }

        public static TinyCsvParserWrapper<TEntity> Wrap(CsvParser<TEntity> csvParser)
        {
            return new TinyCsvParserWrapper<TEntity>(csvParser);
        }
    }
}
