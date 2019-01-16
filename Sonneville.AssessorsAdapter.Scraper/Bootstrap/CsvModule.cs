using Ninject.Modules;
using Sonneville.AssessorsAdapter.Scraper.CSV;
using Sonneville.AssessorsAdapter.Scraper.CSV.Polk;
using TinyCsvParser;

namespace Sonneville.AssessorsAdapter.Scraper.Bootstrap
{
    public class CsvModule : NinjectModule
    {
        public override void Load()
        {
            var csvParser = CreateCsvParser();
            var wrapper = TinyCsvParserWrapper<PolkCountyHouseData>.Wrap(csvParser);

            Bind<CSV.ICsvParser<PolkCountyHouseData>>().ToConstant(wrapper);
        }

        private static CsvParser<PolkCountyHouseData> CreateCsvParser()
        {
            var csvParserOptions = new CsvParserOptions(true, ',');
            return new CsvParser<PolkCountyHouseData>(csvParserOptions, new PolkCountyMapping());
        }
    }
}
