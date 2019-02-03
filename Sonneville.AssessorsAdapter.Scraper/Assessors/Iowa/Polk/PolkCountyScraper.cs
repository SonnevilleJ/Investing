using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;

namespace Sonneville.AssessorsAdapter.Scraper.Assessors.Iowa.Polk
{
    public class PolkCountyScraper : IScraper
    {
        private const string SearchPage =
            "http://web.assess.co.polk.ia.us/cgi-bin/web/tt/form.cgi?tt=simplegeneralform";

        private readonly IWebDriver _webDriver;

        public PolkCountyScraper(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public RealEstateRecord CollectAssessment(string address)
        {
            NavigateToProperty(address);
            Thread.Sleep(3000);

            return new RealEstateRecord
            {
                Location = ParseLocation(),
                Land = ParseLand(),
                Residence = ParseResidence(),
                Assessments = ParseAssessments()
            };
        }

        private LocationRecord ParseLocation()
        {
            var table = FindTable("Location");
            var dictionary = ConstructDictionary(table);

            return new LocationRecord
            {
                Address = ParseString(dictionary, "Address"),
                City = ParseString(dictionary, "City"),
                Zip = ParseInt(dictionary, "Zip"),
            };
        }

        private LandRecord ParseLand()
        {
            var table = FindTable("Land");
            var dictionary = ConstructDictionary(table);

            return new LandRecord
            {
                Acres = double.Parse(ParseString(dictionary, "Acres")),
                SquareFeet = ParseInt(dictionary, "Square Feet"),
                YearPlatted = ParseInt(dictionary, "Year Platted"),
                Shape = ParseString(dictionary, "Shape"),
                Vacancy = ParseString(dictionary, "Vacancy"),
                Topography = ParseString(dictionary, "Topography"),
                Unbuildable = ParseString(dictionary, "Unbuildable") != "No",
            };
        }

        private ResidenceRecord ParseResidence()
        {
            var table = FindTable("Residences");
            var dictionary = ConstructDictionary(table);

            var record = new ResidenceRecord();
            record.Occupancy = ParseString(dictionary, "Occupancy");
            record.ResidenceType = ParseString(dictionary, "Residence Type");
            record.BuildingStyle = ParseString(dictionary, "Building Style");
            record.YearBuilt = ParseInt(dictionary, "Year Built");
            record.NumberOfFamilies = ParseInt(dictionary, "Number Families");
            record.Grade = ParseString(dictionary, "Grade");
            record.Condition = ParseString(dictionary, "Condition");
            record.LivingAreaSquareFootage = new Dictionary<int, int>
            {
                {0, ParseInt(dictionary, "Main Living Area")},
                {1, ParseInt(dictionary, "Upper Living Area")}
            };
            record.UnfinishedBasementSquareFootage = ParseInt(dictionary, "Basement Area");
            record.AttachedGarageSquareFootage = ParseInt(dictionary, "Attached Garage Square Foot");
            record.OpenPorchArea = ParseInt(dictionary, "Open Porch Area");
            record.DeckArea = ParseInt(dictionary, "Deck Area");
            record.VeneerArea = ParseInt(dictionary, "Veneer Area");
            record.Foundation = ParseString(dictionary, "Foundation");
            record.ExteriorWallType = ParseString(dictionary, "Exterior Wall Type");
            record.RoofType = ParseString(dictionary, "Roof Type");
            record.RoofMaterial = ParseString(dictionary, "Roof Material");
            record.Fireplaces = ParseInt(dictionary, "Number Fireplaces");
            record.Heating = ParseString(dictionary, "Heating");
            record.AirConditioning = ParseInt(dictionary, "Air Conditioning");
            record.Bathrooms = ParseInt(dictionary, "Number Bathrooms");
            record.ToiletRooms = ParseInt(dictionary, "Number Toilet Rooms");
            record.Bedrooms = ParseInt(dictionary, "Bedrooms");
            record.Rooms = ParseInt(dictionary, "Rooms");
            return record;
        }

        private IDictionary<int, Assessment> ParseAssessments()
        {
            var table = FindTable("Historical Values");
            var dictionary = ConstructDictionary(table);

            return new Dictionary<int, Assessment>();
        }

        private void NavigateToProperty(string address)
        {
            _webDriver.Navigate().GoToUrl(SearchPage);
            _webDriver.FindElement(By.Id("straddr__address")).SendKeys(address);
            _webDriver.FindElement(By.Name("submit_form")).Click();
        }

        private IWebElement FindTable(string heading)
        {
            var table = _webDriver
                .FindElements(By.TagName("caption"))
                .Single(element => element.Text.Trim().StartsWith(heading))
                .FindElement(By.XPath(".."));
            return table;
        }

        private static Dictionary<string, string> ConstructDictionary(IWebElement table)
        {
            var keys = table
                .FindElements(By.TagName("tr"))
                .Where(tr => tr.FindElements(By.TagName("td")).Any())
                .SelectMany(element => element.FindElements(By.TagName("th")))
                .Select(element => element.Text)
                .ToList();
            var values = table
                .FindElements(By.TagName("tr"))
                .SelectMany(element => element.FindElements(By.TagName("td")))
                .Select(element => element.Text)
                .ToList();
            var dictionary = keys.Zip(values, (key, value) => new KeyValuePair<string, string>(key, value))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return dictionary;
        }

        private static string ParseString(
            IReadOnlyDictionary<string, string> dictionary,
            string key)
        {
            return dictionary[key];
        }

        private static int ParseInt(
            IReadOnlyDictionary<string, string> dictionary,
            string key,
            NumberStyles numberStyles = NumberStyles.Any)
        {
            return int.Parse(dictionary[key], numberStyles);
        }
    }
}