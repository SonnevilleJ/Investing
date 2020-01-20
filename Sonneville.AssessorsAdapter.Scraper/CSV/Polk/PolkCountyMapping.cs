using CsvHelper.Configuration;

namespace Sonneville.AssessorsAdapter.Scraper.CSV.Polk
{
    public class PolkCountyMapping : ClassMap<PolkCountyHouseData>
    {
        public PolkCountyMapping()
        {
            Map(record => record.StreetAddress).Name("Street Address");
            Map(record => record.SalePrice).Name("For Sale Price");
            Map(record => record.RentPricePerMonth).Name("For Rent Price");
            Map(record => record.City).Name("City");
            Map(record => record.State).Name("State");
            Map(record => record.County).Name("County");
            Map(record => record.Zip).Name("Zip");
            Map(record => record.Zillow).Name("Zillow");
            Map(record => record.ZillowTM).Name("Zillow TM");
            Map(record => record.Assessment).Name("Assessment");
            Map(record => record.PolkAsessorTM).Name("Polk Asessor TM");
            Map(record => record.DallasAssessorTM).Name("Dallas Assessor TM");
            Map(record => record.WarrenAssessorTM).Name("Warren Assessor TM");
            Map(record => record.AssessorTM).Name("Assessor TM");
            Map(record => record.AssessmentValue).Name("Assessment Value");
            Map(record => record.LotSize).Name("Lot Size");
            Map(record => record.YearBuilt).Name("Year Built");
            Map(record => record.YearRemodeled).Name("Year Remodeled");
            Map(record => record.Style).Name("Style");
            Map(record => record.Grade).Name("Grade");
            Map(record => record.LivableSqFt).Name("Livable Sq Ft");
            Map(record => record.BasementSqFt).Name("Basement Sq Ft");
            Map(record => record.BasementFinished).Name("Basement Finished");
            Map(record => record.GarageSqFt).Name("Garage Sq Ft");
            Map(record => record.Bedrooms).Name("Bedrooms");
            Map(record => record.BasementBedrooms).Name("Basement Bedrooms");
            Map(record => record.Bathrooms).Name("Bathrooms");
            Map(record => record.ToiletRooms).Name("Toilet rooms");
            Map(record => record.Notes).Name("Notes");
        }
    }
}