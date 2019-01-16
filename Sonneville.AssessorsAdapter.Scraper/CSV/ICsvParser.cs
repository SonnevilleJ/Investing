using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace Sonneville.AssessorsAdapter.Scraper.CSV
{
    public interface ICsvParser<TEntity> where TEntity : class, new()
    {
        ParallelQuery<CsvMappingResult<TEntity>> ReadFromFile(
            string fileName,
            Encoding encoding);

        ParallelQuery<CsvMappingResult<TEntity>> ReadFromString(
            CsvReaderOptions csvReaderOptions,
            string csvData);

        ParallelQuery<CsvMappingResult<TEntity>> Parse(IEnumerable<Row> csvData);
    }
}
