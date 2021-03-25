using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Sonneville.AssessorsAdapter.Scraper.CSV.Polk;

namespace Sonneville.AssessorsAdapter.Scraper.Bootstrap
{
    public class App : IApp
    {
        private readonly IScraper _scraper;

        public App(IScraper scraper)
        {
            _scraper = scraper;
        }

        public void Dispose()
        {
        }

        public void Run(string[] args)
        {
            Console.Write("Enter path of file to read: ");
            const string path = "/home/john/Downloads/data.csv";
            Console.WriteLine($"Reading path: {path}...");

            using (var streamReader = new StreamReader(path, Encoding.UTF8))
            {
                var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
                csvReader.Context.RegisterClassMap<PolkCountyMapping>();

                var realEstateRecords = csvReader.GetRecords<PolkCountyHouseData>()
                    .Select(data => LogData("input", data))
                    .Select(data => _scraper.CollectAssessment(data.StreetAddress))
                    .Select(PolkCountyHouseData.CreateFrom)
                    .Select(data => LogData("output", data));

                var csvWriter = new CsvWriter(new StreamWriter("/home/john/Downloads/out.csv"),
                    CultureInfo.InvariantCulture);
                csvWriter.WriteRecords(realEstateRecords);
            }
        }

        private PolkCountyHouseData LogData(string source, PolkCountyHouseData data)
        {
            Console.WriteLine($"Logging {source} data: {data.StreetAddress} - {data.AssessmentValue}");
            return data;
        }
    }
}