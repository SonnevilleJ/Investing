namespace Sonneville.AssessorsAdapter.Scraper.Assessors.Iowa.Polk
{
    public class Assessment
    {
        public int? Year { get; set; }
        public string Type { get; set; }
        public string Class { get; set; }
        public string Kind { get; set; }
        public int? Land { get; set; }
        public int? Building { get; set; }
        public int? Total => Land + Building;
    }
}
