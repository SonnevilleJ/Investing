using System.Collections.Generic;
using System.Linq;

namespace Sonneville.AssessorsAdapter.Scraper.Assessors.Iowa.Polk
{
    public class ResidenceRecord
    {
        public string Occupancy { get; set; }
        public string ResidenceType { get; set; }
        public string BuildingStyle { get; set; }
        public int YearBuilt { get; set; }
        public int NumberOfFamilies { get; set; }
        public string Grade { get; set; }
        public string Condition { get; set; }
        public IDictionary<int, int> LivingAreaSquareFootage { get; set; }
        public int TotalLivingAreaSquareFootage => LivingAreaSquareFootage.Sum(kvp => kvp.Value);
        public int AttachedGarageSquareFootage { get; set; }
        public int UnattachedGarageSquareFootage { get; set; }
        public int UnfinishedBasementSquareFootage { get; set; }
        public int OpenPorchArea { get; set; }
        public int DeckArea { get; set; }
        public int VeneerArea { get; set; }
        public string Foundation { get; set; }
        public string ExteriorWallType { get; set; }
        public string RoofType { get; set; }
        public string RoofMaterial { get; set; }
        public int Fireplaces { get; set; }
        public string Heating { get; set; }
        public int AirConditioning { get; set; }
        public int Bathrooms { get; set; }
        public int ToiletRooms { get; set; }
        public int Bedrooms { get; set; }
        public int Rooms { get; set; }
    }
}