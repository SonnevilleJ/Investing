using Sonneville.AssessorsAdapter.Scraper.Assessors;

namespace Sonneville.AssessorsAdapter.Scraper.CSV.Polk
{
    public class PolkCountyHouseData
    {
        public string Key { get; set; }
        public string StreetAddress { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RentPricePerMonth { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public int? Zip { get; set; }
        public string Zillow { get; set; }
        public string ZillowTM { get; set; }
        public string Assessment { get; set; }
        public string PolkAsessorTM { get; set; }
        public string DallasAssessorTM { get; set; }
        public string WarrenAssessorTM { get; set; }
        public string AssessorTM { get; set; }
        public decimal? AssessmentValue { get; set; }
        public int? LotSize { get; set; }
        public int? YearBuilt { get; set; }
        public int? YearRemodeled { get; set; }
        public string Style { get; set; }
        public string Grade { get; set; }
        public int? LivableSqFt { get; set; }
        public int? BasementSqFt { get; set; }
        public int? BasementFinished { get; set; }
        public int? GarageSqFt { get; set; }
        public int? Bedrooms { get; set; }
        public int? BasementBedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? ToiletRooms { get; set; }
        public string Notes { get; set; }

        public static PolkCountyHouseData CreateFrom(RealEstateRecord record)
        {
            return new PolkCountyHouseData
            {
                StreetAddress = record.Location.Address,
                City = record.Location.City,
                Zip = record.Location.Zip,
                County = record.Location.County,
                State = record.Location.State,
                LotSize = record.Land.SquareFeet,
                YearBuilt = record.Residence.YearBuilt,
                LivableSqFt = record.Residence.TotalLivingAreaSquareFootage,
                BasementSqFt = record.Residence.UnfinishedBasementSquareFootage,
                BasementFinished = record.Residence.LivingAreaSquareFootage.TryGetValue(-1, out var basementFinished)
                    ? basementFinished
                    : 0,
                GarageSqFt = record.Residence.AttachedGarageSquareFootage,
                Bedrooms = record.Residence.Bedrooms,
                BasementBedrooms = record.Residence.BasementBedrooms,
                Bathrooms = record.Residence.Bathrooms,
                ToiletRooms = record.Residence.ToiletRooms,
                AssessmentValue = record.CurrentValue,
            };
        }
    }
}