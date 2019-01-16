using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sonneville.AssessorsAdapter.Scraper.Bootstrap;
using Sonneville.AssessorsAdapter.Scraper.CSV;
using Sonneville.AssessorsAdapter.Scraper.CSV.Polk;
using TinyCsvParser.Mapping;

namespace Sonneville.AssessorsAdapter.Scraper.Test.Bootstrap
{
    [TestFixture]
    public class AppTests
    {
        [SetUp]
        public void Setup()
        {
            _data = new List<PolkCountyHouseData>
            {
                new PolkCountyHouseData()
            };

            _mockCsvParser = new Mock<ICsvParser<PolkCountyHouseData>>();

            _app = new App(_mockCsvParser.Object);
        }

        private List<PolkCountyHouseData> _data;
        private Mock<ICsvParser<PolkCountyHouseData>> _mockCsvParser;
        private App _app;

        private static CsvMappingResult<PolkCountyHouseData> CreateCsvMappingResult(PolkCountyHouseData result)
        {
            return new CsvMappingResult<PolkCountyHouseData>
            {
                Error = null,
                Result = result
            };
        }

        [Test]
        public void ShouldRun()
        {
            var results = _data.Select(CreateCsvMappingResult)
                .AsParallel();
            _mockCsvParser.Setup(parser => parser.ReadFromFile("test.csv", Encoding.Unicode))
                .Returns(results);

            _app.Run(new string[0]);
        }
    }
}
