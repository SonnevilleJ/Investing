using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenQA.Selenium;

namespace Sonneville.AssessorsAdapter.Scraper.Assessors.Iowa.Polk
{
    public sealed class PolkCountyScraper : IScraper
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
            QueryForAddress(address);
            return new RealEstateRecord
            {
                Location = ParseLocation(),
                Land = ParseLand(),
                Residence = ParseResidence(),
                Assessments = ParseAssessments()
            };
        }

        public void Dispose()
        {
            _webDriver?.Dispose();
        }

        private LocationRecord ParseLocation()
        {
            var table = FindTable("Location");
            var dictionary = ConstructDictionaryFromInterleavedData(table);
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
            var dictionary = ConstructDictionaryFromInterleavedData(table);
            return new LandRecord
            {
                Acres = double.Parse(dictionary["Acres"]),
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
            var dictionary = ConstructDictionaryFromInterleavedData(table);
            return new ResidenceRecord
            {
                Occupancy = ParseString(dictionary, "Occupancy"),
                ResidenceType = ParseString(dictionary, "Residence Type"),
                BuildingStyle = ParseString(dictionary, "Building Style"),
                YearBuilt = ParseInt(dictionary, "Year Built"),
                NumberOfFamilies = ParseInt(dictionary, "Number Families"),
                Grade = ParseString(dictionary, "Grade"),
                Condition = ParseString(dictionary, "Condition"),
                LivingAreaSquareFootage = new Dictionary<int, int?>
                {
                    {-1, ParseInt(dictionary, "Total Basement Finish")},
                    {0, ParseInt(dictionary, "Main Living Area")},
                    {1, ParseInt(dictionary, "Upper Living Area")}
                },
                UnfinishedBasementSquareFootage = ParseInt(dictionary, "Basement Area"),
                AttachedGarageSquareFootage = ParseInt(dictionary, "Attached Garage Square Foot"),
                OpenPorchArea = ParseInt(dictionary, "Open Porch Area"),
                DeckArea = ParseInt(dictionary, "Deck Area"),
                VeneerArea = ParseInt(dictionary, "Veneer Area"),
                Foundation = ParseString(dictionary, "Foundation"),
                ExteriorWallType = ParseString(dictionary, "Exterior Wall Type"),
                RoofType = ParseString(dictionary, "Roof Type"),
                RoofMaterial = ParseString(dictionary, "Roof Material"),
                Fireplaces = ParseInt(dictionary, "Number Fireplaces"),
                Heating = ParseString(dictionary, "Heating"),
                AirConditioning = ParseInt(dictionary, "Air Conditioning"),
                Bathrooms = ParseInt(dictionary, "Number Bathrooms"),
                ToiletRooms = ParseInt(dictionary, "Number Toilet Rooms"),
                Bedrooms = ParseInt(dictionary, "Bedrooms"),
                Rooms = ParseInt(dictionary, "Rooms")
            };
        }

        private List<Assessment> ParseAssessments()
        {
            var table = FindTable("Historical Values");
            var columns = table
                .FindElement(By.TagName("thead"))
                .FindElements(By.TagName("th"))
                .Select(th => th.Text)
                .Select(KeyValuePair.Create)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return table
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"))
                .Select(tr => CreateAssessment(tr, columns))
                .ToList();
        }

        private Assessment CreateAssessment(IWebElement tr, IReadOnlyDictionary<string, int> columns)
        {
            var values = tr
                .FindElements(By.TagName("td"))
                .Select(td => td.Text)
                .ToList();
            return new Assessment
            {
                Land = ParseInt(values[columns["Land"]]),
                Year = ParseInt(values[columns["Yr"]]),
                Building = ParseInt(values[columns["Bldg"]]),
                Class = values[columns["Class"]],
                Kind = values[columns["Kind"]],
                Type = values[columns["Type"]]
            };
        }

        private void QueryForAddress(string address)
        {
            _webDriver.Navigate().GoToUrl(SearchPage);
            _webDriver.FindElement(By.Id("straddr__address")).SendKeys(address);
            _webDriver.FindElement(By.Name("submit_form")).Click();
        }

        private IWebElement FindTable(string heading)
        {
            return _webDriver
                .FindElements(By.TagName("caption"))
                .Single(element => element.Text.Trim().StartsWith(heading))
                .FindElement(By.XPath(".."));
        }

        private static Dictionary<string, string> ConstructDictionaryFromInterleavedData(IWebElement table)
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
            return keys.Zip(values, KeyValuePair.Create)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static int? ParseInt(Dictionary<string, string> dictionary, string key)
        {
            return dictionary.TryGetValue(key, out var stringValue)
                ? ParseInt(stringValue)
                : (int?) null;
        }

        private static int ParseInt(string value, NumberStyles numberStyles = NumberStyles.Any)
        {
            return int.Parse(value, numberStyles);
        }

        private static string ParseString(Dictionary<string, string> dictionary, string key)
        {
            return dictionary.TryGetValue(key, out var stringValue)
                ? stringValue
                : default(string);
        }
    }
}
