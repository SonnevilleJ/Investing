using System;
using Sonneville.AssessorsAdapter.Scraper.Assessors.Iowa.Polk;

namespace Sonneville.AssessorsAdapter.Scraper
{
    public interface IScraper : IDisposable
    {
        RealEstateRecord CollectAssessment(string address);
    }
}