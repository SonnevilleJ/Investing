using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Sonneville.AssessorsAdapter.Scraper.Assessors.Iowa.Polk;

namespace Sonneville.AssessorsAdapter.Scraper.Test.Assessors.Iowa.Polk
{
    [TestFixture]
    public class PolkCountyScraperTests
    {
        [SetUp]
        public void Setup()
        {
            _chromeDriver = CreateWebDriver();
            _scraper = new PolkCountyScraper(_chromeDriver);
        }

        [TearDown]
        public void Teardown()
        {
            _chromeDriver?.Dispose();
            _scraper?.Dispose();
        }

        private PolkCountyScraper _scraper;
        private IWebDriver _chromeDriver;

        private IWebDriver CreateWebDriver()
        {
            var chromeOptions = new ChromeOptions();
            var chromeDriverDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new ChromeDriver(chromeDriverDirectory, chromeOptions);
        }

        [Test]
        public void ShouldDisposeWebDriver()
        {
            var mockWebDriver = new Mock<IWebDriver>();
            new PolkCountyScraper(mockWebDriver.Object).Dispose();

            mockWebDriver.Verify(webDriver => webDriver.Dispose());
        }

        [Test]
        public void ShouldParseHouseWithFinishedBasement()
        {
            var record = _scraper.CollectAssessment("1513 NW Prairie Creek Dr");

            Assert.AreEqual(1100, record.Residence.LivingAreaSquareFootage[-1]);
        }

        [Test]
        public void ShouldParseHouseWithMissingData()
        {
            var record = _scraper.CollectAssessment("1610 63rd St");

            Assert.IsNull(record.Land.YearPlatted);
        }

        [Test]
        public void ShouldParseJohnsHouse()
        {
            var record = _scraper.CollectAssessment("5166 Raintree Dr");

            Assert.AreEqual("5166 RAINTREE DR", record.Location.Address);
            Assert.AreEqual("WEST DES MOINES", record.Location.City);
            Assert.AreEqual(50265, record.Location.Zip);

            Assert.AreEqual(0.287, record.Land.Acres, 0.0001);
            Assert.AreEqual(12501, record.Land.SquareFeet);
            Assert.AreEqual(1992, record.Land.YearPlatted);

            Assert.AreEqual(1995, record.Residence.YearBuilt);
            Assert.AreEqual("3-05", record.Residence.Grade);
            Assert.AreEqual(1054, record.Residence.LivingAreaSquareFootage[0]);
            Assert.AreEqual(788, record.Residence.LivingAreaSquareFootage[1]);
            Assert.AreEqual(440, record.Residence.AttachedGarageSquareFootage);
            Assert.AreEqual(60, record.Residence.OpenPorchArea);
            Assert.AreEqual(168, record.Residence.DeckArea);
            Assert.AreEqual("Poured Concrete", record.Residence.Foundation);
            Assert.AreEqual("Asphalt Shingle", record.Residence.RoofMaterial);
            Assert.AreEqual(1, record.Residence.Fireplaces);
            Assert.AreEqual("Gas Forced Air", record.Residence.Heating);
            Assert.AreEqual(100, record.Residence.AirConditioning);
            Assert.AreEqual(2, record.Residence.Bathrooms);
            Assert.AreEqual(1, record.Residence.ToiletRooms);
            Assert.AreEqual(3, record.Residence.Bedrooms);
            Assert.AreEqual(7, record.Residence.Rooms);

            Assert.AreEqual(253000, record.Assessments.Single(assessment => assessment.Year == 2017).Total);

            Assert.AreEqual(253000, record.CurrentValue);
        }
    }
}
