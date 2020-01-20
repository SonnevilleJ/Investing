using System.Collections.Generic;
using System.Linq;

namespace Sonneville.AssessorsAdapter.Scraper.Assessors
{
    public class RealEstateRecord
    {
        public LocationRecord Location { get; set; }
        public LandRecord Land { get; set; }
        public ResidenceRecord Residence { get; set; }
        public List<Assessment> Assessments { get; set; }

        public int? CurrentValue => Assessments
            .First().Total;
    }
}
