using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Sonneville.AssessorsAdapter.Scraper.Assessors;
using Sonneville.AssessorsAdapter.Scraper.Bootstrap;
using Sonneville.AssessorsAdapter.Scraper.CSV.Polk;

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
                new PolkCountyHouseData {StreetAddress = "house 1"},
                new PolkCountyHouseData {StreetAddress = "house 2"},
            };

            _scraperMock = new Mock<IScraper>();
            foreach (var pair in _data.ToDictionary(data => data, Convert))
                _scraperMock.Setup(scraper => scraper.CollectAssessment(pair.Key.StreetAddress))
                    .Returns(pair.Value);

            _app = new App(_scraperMock.Object);
        }

        private RealEstateRecord Convert(PolkCountyHouseData data)
        {
            return new RealEstateRecord
            {
                Location = new LocationRecord {Address = data.StreetAddress},
                Assessments = new List<Assessment>
                {
                    new Assessment {Land = 10000, Building = 100000},
                },
            };
        }

        private List<PolkCountyHouseData> _data;
        private Mock<IScraper> _scraperMock;
        private App _app;

        [Test]
        public void ShouldParseProperties()
        {
            _app.Run(new string[0]);

            _data.ForEach(data => _scraperMock.Verify(scraper => scraper.CollectAssessment(data.StreetAddress)));
        }
    }
}