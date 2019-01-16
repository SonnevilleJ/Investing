using System.Linq;
using System.Text;
using Sonneville.AssessorsAdapter.Scraper.CSV;
using Sonneville.AssessorsAdapter.Scraper.CSV.Polk;

namespace Sonneville.AssessorsAdapter.Scraper.Bootstrap
{
    public class App : IApp
    {
        private readonly ICsvParser<PolkCountyHouseData> _csvParser;

        public App(ICsvParser<PolkCountyHouseData> csvParser)
        {
            _csvParser = csvParser;
        }

        public void Dispose()
        {
        }

        public void Run(string[] args)
        {
            var list = _csvParser.ReadFromFile("test.csv", Encoding.Unicode)
                .Where(r => r.IsValid)
                .Select(r => r.Result)
                .ToList();
        }
    }
}
