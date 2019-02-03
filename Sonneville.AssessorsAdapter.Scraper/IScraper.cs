using Sonneville.AssessorsAdapter.Scraper.Assessors.Iowa.Polk;

namespace Sonneville.AssessorsAdapter.Scraper
{
    public interface IScraper
    {
        RealEstateRecord CollectAssessment(string address);
    }
}